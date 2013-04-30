//
// Created by Owen Evans on 24/04/13.
// Copyright (c) 2013 Owen Evans. All rights reserved.
//
// To change the template use AppCode | Preferences | File Templates.
//


#import <Foundation/Foundation.h>
#import <CoreLocation/CoreLocation.h>
@class DeviceRegistration;
@class Event;


@interface Server : NSObject
-(void)registerDevice:(DeviceRegistration *)device;

- (void)lookupEvent:(NSString *)code;

- (void)setCredentialsToGuid:(NSString *)guid Token:(NSString *)token;

- (void)loadEventsCloseTo:(CLLocationCoordinate2D)location;

- (void)postPhoto:(NSData *)data ToEvent:(Event*) event;

- (void)postEvent:(Event *)event;

@end