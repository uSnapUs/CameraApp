//
// Created by Owen Evans on 24/04/13.
// Copyright (c) 2013 Owen Evans. All rights reserved.
//
// To change the template use AppCode | Preferences | File Templates.
//


#import <Foundation/Foundation.h>
#import "ATModel.h"

@class User;


@interface DeviceRegistration : ATModel <MTLJSONSerializing>

@property(nonatomic, copy) NSString *guid;
@property(nonatomic, copy) NSString *name;
@property (nonatomic, copy, readonly) NSString *token;
@property (nonatomic,copy) User *user;
@end