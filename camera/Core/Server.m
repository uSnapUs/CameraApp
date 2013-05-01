//
// Created by Owen Evans on 24/04/13.
// Copyright (c) 2013 Owen Evans. All rights reserved.
//
// To change the template use AppCode | Preferences | File Templates.
//


#import "Server.h"
#import "DeviceRegistration.h"
#import "AFNetworking.h"
#import "SVProgressHUD.h"
#import "Event.h"


@implementation Server {

    AFHTTPClient *client;
}
-(id) init{
    self = [super init];
    if(self){
        [self setupNetworkingClient];
    }
    return self;
}

- (void)setupNetworkingClient {
    client = [[AFHTTPClient alloc] initWithBaseURL:[NSURL URLWithString:kBaseURL]];
   [client setParameterEncoding:AFJSONParameterEncoding];
    [client registerHTTPOperationClass:[AFJSONRequestOperation class]];
    [client setDefaultHeader:@"Accept" value:@"application/json"];

}

- (void)registerDevice:(DeviceRegistration *)device {
    DDLogVerbose(@"registering device with server");
    [client postPath:@"/devices" parameters:[MTLJSONAdapter JSONDictionaryFromModel:device] success:^(AFHTTPRequestOperation *operation, id responseObject) {
        DDLogVerbose(@"got response from server %@",((NSDictionary*)responseObject));
        NSError *error;



        DeviceRegistration *deviceRegistration=[MTLJSONAdapter modelOfClass:[DeviceRegistration class] fromJSONDictionary:((NSDictionary *)responseObject) error:&error];

       if(!error){

           if([device _id]){
               [device updateWithSuccessBlock:^{
                   DDLogVerbose(@"saved update to device, raising notification");
                   [[NSNotificationCenter defaultCenter] postNotificationName:kDeviceUpdateNotification object:nil];
               } withErrorBlock:^(NSError *saveError) {
                   DDLogError(@"unable to save device %@",saveError);

               }];
           }
           else{
               [deviceRegistration createWithSuccessBlock:^{
                   DDLogVerbose(@"saved new device, raising notification");
                   [[NSNotificationCenter defaultCenter] postNotificationName:kDeviceUpdateNotification object:nil];
               } withErrorBlock:^(NSError *updateError) {
                   DDLogError(@"unable to update device %@",updateError);

               }];
           }
       }
        else{
           DDLogError(@"unable to deserialize device: %@", error);
       }
    } failure:^(AFHTTPRequestOperation *operation, NSError *error) {
        DDLogError(@"unable to register device with server %@",error);

    }];
}

- (void)lookupEvent:(NSString *)code {
    [SVProgressHUD showWithMaskType:SVProgressHUDMaskTypeGradient];
    [client getPath:[NSString stringWithFormat:@"/event/%@",code]parameters:nil success:^(AFHTTPRequestOperation *operation,id responseObject){
        DDLogVerbose(@"got event: %@", ((NSDictionary*)responseObject));
        NSError *error;
        Event *event =[MTLJSONAdapter modelOfClass:[Event class] fromJSONDictionary:((NSDictionary*)responseObject) error:&error];

        [SVProgressHUD dismiss];
        if(error)
        {
          DDLogError(@"error creating event %@", error);
        }
        else{

            [[NSNotificationCenter defaultCenter] postNotificationName:kEventFoundNotification object:self userInfo:[NSDictionary dictionaryWithObject:event forKey:@"event"]];
        }
    }failure:^(AFHTTPRequestOperation *operation, NSError *error) {
        DDLogError(@"unable to lookup event: %@", error);
        [SVProgressHUD showErrorWithStatus:@"unable to find event"];

    }];
}

- (void)setCredentialsToGuid:(NSString *)guid Token:(NSString *)token {
    [client setAuthorizationHeaderWithUsername:guid password:token];
}

- (void)loadEventsCloseTo:(CLLocationCoordinate2D)location {
    [client getPath:[NSString stringWithFormat:@"/events/by_location"] parameters:@{@"latitude":[[NSNumber alloc] initWithDouble:location.latitude], @"longitude":[[NSNumber alloc] initWithDouble: location.longitude]} success:^(AFHTTPRequestOperation *operation,id responseObject){
        NSArray *eventDictionaries = ((NSArray*)responseObject);
        NSMutableArray *events = [[NSMutableArray alloc]init];
        DDLogVerbose(@"got events: %i", [eventDictionaries count]);
        for (NSDictionary *dictionary in eventDictionaries) {
            NSError *error;
            [events addObject:[MTLJSONAdapter modelOfClass:[Event class] fromJSONDictionary:dictionary error:&error]];
            if(error) {
                DDLogError(@"unable to create event %@", error);
            }

        }


        [SVProgressHUD dismiss];
        [[NSNotificationCenter defaultCenter] postNotificationName:kEventsForLocationLookedUp object:self userInfo:@{@"events":events}];

    }failure:^(AFHTTPRequestOperation *operation, NSError *error) {
        DDLogError(@"unable to lookup event: %@", error);
        [SVProgressHUD dismiss];
    }];

}

- (void)postPhoto:(NSData *)data ToEvent:(Event *)event {
    NSURLRequest *request= [client multipartFormRequestWithMethod:@"POST" path:[NSString stringWithFormat:@"/event/%@/photos",[event code]] parameters:nil constructingBodyWithBlock:^(id <AFMultipartFormData> formData) {
        [formData appendPartWithFileData:data name:@"photo" fileName:@"photo.jpg" mimeType:@"image/jpeg"];
    }];
    AFJSONRequestOperation *operation =  [[AFJSONRequestOperation alloc] initWithRequest:request];

    [operation setUploadProgressBlock:^(NSUInteger bytesWritten, long long int totalBytesWritten, long long int totalBytesExpectedToWrite) {
        float percentUploaded =(float)totalBytesWritten/(float)totalBytesExpectedToWrite;
        DDLogVerbose(@"uploading photo percent done: %f, total written: %llu, total expected: %llu", percentUploaded,totalBytesWritten,totalBytesExpectedToWrite);
        if(percentUploaded<1){
        [SVProgressHUD showProgress:percentUploaded status:@"uploding photo" maskType:SVProgressHUDMaskTypeGradient];
        }
        else{
            [SVProgressHUD showProgress:percentUploaded status:@"processing photo" maskType:SVProgressHUDMaskTypeGradient];
        }
    }];
    [operation setCompletionBlockWithSuccess:^(AFHTTPRequestOperation *uploadOperation, id responseObject) {
        [[NSNotificationCenter defaultCenter] postNotificationName:kEventUpdated object:self userInfo:@{@"event":event}];
        DDLogVerbose(@"uploaded photo");
        [SVProgressHUD showSuccessWithStatus:@"uploaded"];
    } failure:^(AFHTTPRequestOperation *uploadOperation, NSError *error) {
        DDLogError(@"error uploading %@", error);
        [SVProgressHUD showErrorWithStatus:@"failed to upload photo"];
    }];

    [operation start];


}

- (void)postEvent:(Event *)event {
    [SVProgressHUD showWithStatus:@"creating event" maskType:SVProgressHUDMaskTypeGradient];
    DDLogVerbose(@"posting event %@", event);
       [client postPath:@"/events" parameters:[MTLJSONAdapter JSONDictionaryFromModel:event] success:^(AFHTTPRequestOperation *operation, id responseObject) {

           DDLogVerbose(@"post returned event event: %@", ((NSDictionary*)responseObject));
           NSError *error;
           Event *createdEvent =[MTLJSONAdapter modelOfClass:[Event class] fromJSONDictionary:((NSDictionary*)responseObject) error:&error];

           if(error)
           {
               DDLogError(@"error creating event %@", error);
               [SVProgressHUD showErrorWithStatus:@"unable to save event"];

           }
           else{
               [SVProgressHUD showSuccessWithStatus:@"event created"];
               [[NSNotificationCenter defaultCenter] postNotificationName:kEventFoundNotification object:self userInfo:[NSDictionary dictionaryWithObject:createdEvent forKey:@"event"]];
           }


       } failure:^(AFHTTPRequestOperation *operation, NSError *error) {
           DDLogError(@"unable to create event %@",error);
           [SVProgressHUD showErrorWithStatus:@"unable to save event"];

       }];



}
@end