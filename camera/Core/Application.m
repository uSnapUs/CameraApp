//
// Created by Owen Evans on 24/04/13.
// Copyright (c) 2013 Owen Evans. All rights reserved.
//
// To change the template use AppCode | Preferences | File Templates.
//


#import <ActiveTouch/ATDatabaseContainer.h>
#import "Application.h"
#import "Server.h"
#import "DeviceRegistration.h"
#import <CoreLocation/CoreLocation.h>


@implementation Application {


@private
    Server *_server;
    DeviceRegistration *_currentDevice;



}

static Application *sharedInstance;


@synthesize currentDevice = _currentDevice;

-(id) init{
    self = [super init];
    if(!self){
        return nil;
    }
    [self setupNotifications] ;
    return self;
}



- (NSString *)generateUuidString
{
    CFUUIDRef uuid = CFUUIDCreate(kCFAllocatorDefault);

    NSString *uuidString = (__bridge NSString *)CFUUIDCreateString(kCFAllocatorDefault, uuid);

    CFRelease(uuid);

    return uuidString;
}

- (Server *)server {
    if(!_server)
    {
        _server = [[Server alloc]init];
    }
    return _server;
}

- (void)setServer:(Server *)server {
    _server = server;
}

- (void)setupNotifications {
    [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(onApplicationLoaded) name:kApplicationLoadedNotification object:nil];
    [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(setCurrentDevice) name:kDeviceUpdateNotification object:nil];

}
-(void) onApplicationLoaded{
    DDLogVerbose(@"setting up server");
    NSString *dbName = kDBName;
    [[ATDatabaseContainer sharedInstance] openDatabaseWithName:dbName];

    DDLogVerbose(@"finished setting up server");
    [self setCurrentDevice];
}
-(void)dealloc {
    [[NSNotificationCenter defaultCenter] removeObserver:self];
}

+(Application*)sharedInstance {
    return sharedInstance;
}

- (void)setCurrentDevice {
    DDLogVerbose(@"loading current device");
    //load all devices (there should only be one)
    [DeviceRegistration allWithSuccessBlock:^(NSArray *devices){
        DDLogVerbose(@"successfully interogated database for devices");
        if(!devices||[devices count]==0){
            DDLogVerbose(@"no saved devices so creating one");
            DeviceRegistration *device = [[DeviceRegistration alloc]init];
            device.guid = [self generateUuidString];
            device.name = [[UIDevice currentDevice] name];
            [[self server] registerDevice:device];
        }
        else
        {
            DDLogVerbose(@"got device from db so setting as current device");
            [self setCurrentDevice:[devices objectAtIndex:0]];
            [[self server]setCredentialsToGuid:[[self currentDevice] guid]Token:[[self currentDevice] token]];
        }
    } withErrorBlock:
            ^(NSError *error) {
                DDLogError(@"error interogating database %@",error);

            }];
}

+(void)initialize {
    static BOOL initialized = NO;
    if(!initialized)
    {
        initialized = YES;
        sharedInstance = [[Application alloc] init];
    }
}

- (void)lookupEventByCode:(NSString *)code {
    [[self server]lookupEvent:code];

}

- (void)loadEventsCloseTo:(CLLocationCoordinate2D)location {
    [[self server]loadEventsCloseTo:location];

}
@end