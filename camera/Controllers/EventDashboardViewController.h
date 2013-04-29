//
// Created by Owen Evans on 26/04/13.
// Copyright (c) 2013 Owen Evans. All rights reserved.
//
// To change the template use AppCode | Preferences | File Templates.
//


#import <Foundation/Foundation.h>
#import "DLCImagePickerController.h"


@class Event;


@interface EventDashboardViewController : UIViewController <UITableViewDelegate,UITableViewDataSource, DLCImagePickerDelegate>
@property (weak, nonatomic) IBOutlet UITableView *streamView;

@property (weak, nonatomic) IBOutlet UILabel *eventTitleLabel;
@property (strong, readwrite) Event *event;
- (IBAction)goToMainMenu:(id)sender;
- (IBAction)showPickerView:(id)sender;

@end