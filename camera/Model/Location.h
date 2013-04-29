//
// Created by Owen Evans on 26/04/13.
// Copyright (c) 2013 Owen Evans. All rights reserved.
//
// To change the template use AppCode | Preferences | File Templates.
//


#import <Foundation/Foundation.h>
#import <Mantle/MTLJSONAdapter.h>
#import <ActiveTouch/ATModel.h>


@interface Location : ATModel
    @property (nonatomic,copy) NSArray *coordinates;
    @property (nonatomic,copy) NSString *type;
    -(double) latitude;
    -(double) longitude;
@end