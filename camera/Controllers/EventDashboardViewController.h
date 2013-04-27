//
// Created by Owen Evans on 26/04/13.
// Copyright (c) 2013 Owen Evans. All rights reserved.
//
// To change the template use AppCode | Preferences | File Templates.
//


#import <Foundation/Foundation.h>

@class Event;


@interface EventDashboardViewController : UIViewController <UITableViewDelegate,UITableViewDataSource>

@property (strong, readwrite) Event *event;

@end