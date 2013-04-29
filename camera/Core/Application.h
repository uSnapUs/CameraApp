//
// Created by Owen Evans on 24/04/13.
// Copyright (c) 2013 Owen Evans. All rights reserved.
//
// To change the template use AppCode | Preferences | File Templates.
//


#import <Foundation/Foundation.h>
#import <CoreLocation/CoreLocation.h>

@class Server;
@class DeviceRegistration;
@class Event;


@interface Application : NSObject

@property(nonatomic, strong) Server *server;

@property(nonatomic, strong) DeviceRegistration *currentDevice;

-(void) setupNotifications;

- (void)onApplicationLoaded;

+(Application*)sharedInstance;

- (void)setCurrentDevice;


- (void)lookupEventByCode:(NSString *)code;

- (void)loadEventsCloseTo:(CLLocationCoordinate2D)location;

- (void)uploadPhoto:(NSData *)data ToEvent:(Event *)event;

- (void)saveEvent:(Event *)event;
@end