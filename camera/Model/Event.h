//
// Created by Owen Evans on 26/04/13.
// Copyright (c) 2013 Owen Evans. All rights reserved.
//
// To change the template use AppCode | Preferences | File Templates.
//


#import <Foundation/Foundation.h>
#import <Mantle/MTLJSONAdapter.h>
#import <ActiveTouch/ATModel.h>
#import <MapKit/MapKit.h>

@class Location;


@interface Event : ATModel <MTLJSONSerializing, MKAnnotation>
@property(nonatomic, copy) NSString *address;
@property(nonatomic, copy) NSString *code;
@property (nonatomic, copy) NSDate *createdAt;
@property (nonatomic, copy) NSDate *creation_date_utc;
@property (nonatomic, copy) NSDate *end_date;

@property (nonatomic, copy) NSDate *start_date;

@property (nonatomic) BOOL is_public;
@property (nonatomic,copy) Location *location;
@property (nonatomic,copy) NSString *name;
@property (nonatomic,copy) NSDate *updatedAt;
@property (nonatomic, copy) NSString *id;
@property (nonatomic,copy) NSArray *photos;

@end