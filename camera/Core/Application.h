//
// Created by Owen Evans on 24/04/13.
// Copyright (c) 2013 Owen Evans. All rights reserved.
//
// To change the template use AppCode | Preferences | File Templates.
//
#define kDBName @"usnapus_db"
#define kApplicationLoadedNotification @"uSnapUs_ApplicationLoadedNotification"

#import <Foundation/Foundation.h>


@interface Application : NSObject

-(void) setupNotifications;

+(Application*)sharedInstance;


@end