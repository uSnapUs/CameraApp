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
#import "Event.h"
#import "AppDelegate.h"
#import "User.h"
#import "Photo.h"


@implementation Application {


@private
    Server *_server;
    DeviceRegistration *_currentDevice;


}

static Application *sharedInstance;


@synthesize currentDevice = _currentDevice;

- (id)init {
    self = [super init];
    if (!self) {
        return nil;
    }
    [self setupNotifications];
    return self;
}


- (NSString *)generateUuidString {
    CFUUIDRef uuid = CFUUIDCreate(kCFAllocatorDefault);

    NSString *uuidString = (__bridge NSString *) CFUUIDCreateString(kCFAllocatorDefault, uuid);

    CFRelease(uuid);

    return uuidString;
}

- (Server *)server {
    if (!_server) {
        _server = [[Server alloc] init];
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

- (void)onApplicationLoaded {
    DDLogVerbose(@"setting up server");
    NSString *dbName = kDBName;
    [[ATDatabaseContainer sharedInstance] openDatabaseWithName:dbName];

    DDLogVerbose(@"finished setting up server");
    [self setCurrentDevice];
}

- (void)dealloc {
    [[NSNotificationCenter defaultCenter] removeObserver:self];
}

+ (Application *)sharedInstance {
    return sharedInstance;
}

- (void)setCurrentDevice {
    DDLogVerbose(@"loading current device");
    //load all devices (there should only be one)
    [DeviceRegistration allWithSuccessBlock:^(NSArray *devices) {
        DDLogVerbose(@"successfully interogated database for devices");
        if (!devices || [devices count] == 0) {
            DDLogVerbose(@"no saved devices so creating one");
            DeviceRegistration *device = [[DeviceRegistration alloc] init];
            device.guid = [self generateUuidString];
            device.name = [[UIDevice currentDevice] name];
            [[self server] registerDevice:device];
        }
        else {
            DDLogVerbose(@"got device from db so setting as current device");
            [self setCurrentDevice:[devices objectAtIndex:0]];
            DDLogVerbose(@"set device to %@", [self currentDevice]);
            [[self server] setCredentialsToGuid:[[self currentDevice] guid] Token:[[self currentDevice] token]];
            if ([[self currentDevice] user] && (![[[self currentDevice] user] serverId])) {
                [[self server] registerDevice:[self currentDevice]];
            }
        }
    }                        withErrorBlock:
            ^(NSError *error) {
                DDLogError(@"error interogating database %@", error);

            }];
}

+ (void)initialize {
    static BOOL initialized = NO;
    if (!initialized) {
        initialized = YES;
        sharedInstance = [[Application alloc] init];
    }
}

- (void)lookupEventByCode:(NSString *)code {
    [[self server] lookupEvent:code];

}

- (void)loadEventsCloseTo:(CLLocationCoordinate2D)location {
    [[self server] loadEventsCloseTo:location];

}

- (void)uploadPhoto:(NSData *)data ToEvent:(Event *)event {
    [[self server] postPhoto:data ToEvent:event];

}

- (void)saveEvent:(Event *)event {
    [[self server] postEvent:event];

}

- (BOOL)isAuthenticated {
    return [[self currentDevice] user] && [[[self currentDevice] user] facebookId] != nil;
}

- (void)login {
    AppDelegate *appDelegate = (AppDelegate *) [[UIApplication sharedApplication] delegate];
    // The user has initiated a login, so call the openSession method
    // and show the login UX if necessary.
    [appDelegate openSessionWithAllowLoginUI:YES];

}

- (void)loginWithUserId:(NSString *)facebookId Name:(NSString *)name Email:(NSString *)email {
    if ([[self currentDevice] user]) {
        [[[self currentDevice] user] delete:self];
    }
    User *user = [[User alloc] init];

    [user setEmail:email];
    [user setName:name];
    [user setFacebookId:facebookId];
    [[self currentDevice] setUser:user];
    [[self server] registerDevice:[self currentDevice]];
    [[NSNotificationCenter defaultCenter] postNotificationName:kUserLoggedIn object:self];

}

- (void)registerLikeForPhoto:(Photo *)photo inEvent:(Event *)event {
    [[self server]registerLikeForPhoto:photo inEvent:event];

}

- (void)removeLikeForPhoto:(Photo *)photo inEvent:(Event *)event {
    [[self server]removeLikeForPhoto:photo inEvent:event];
}
@end