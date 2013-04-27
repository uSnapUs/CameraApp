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

       DeviceRegistration *deviceRegistration= [DeviceRegistration modelWithDictionary:((NSDictionary*)responseObject)error:&error];
       if(!error){
           [deviceRegistration createWithSuccessBlock:^{
               DDLogVerbose(@"saved device, raising notification");
               [[NSNotificationCenter defaultCenter] postNotificationName:kDeviceUpdateNotification object:nil];
           } withErrorBlock:^(NSError *error) {
               DDLogError(@"unable to save device %@",error);

           }];
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
@end