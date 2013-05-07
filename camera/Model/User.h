//
// Created by Owen Evans on 6/05/13.
// Copyright (c) 2013 Owen Evans. All rights reserved.
//
// To change the template use AppCode | Preferences | File Templates.
//


#import <Foundation/Foundation.h>
#import <ActiveTouch/ATModel.h>


@interface User : ATModel
    @property NSString *email;
    @property NSString *facebookId;
    @property NSString *name;
    @property (readonly) NSString *serverId;
@end