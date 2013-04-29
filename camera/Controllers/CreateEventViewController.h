//
// Created by Owen Evans on 29/04/13.
// Copyright (c) 2013 Owen Evans. All rights reserved.
//
// To change the template use AppCode | Preferences | File Templates.
//


#import <Foundation/Foundation.h>
#import <MapKit/MapKit.h>
#import "CKCalendarView.h"

@class CreateEventView;


@interface CreateEventViewController : UIViewController <MKMapViewDelegate, CKCalendarDelegate,UITextFieldDelegate>

- (IBAction)goToMainMenu:(id)sender;
- (IBAction)goToChooseLocation:(id)sender;
- (IBAction)showEventDateChooser:(id)sender;
- (IBAction)togglePublic:(id)sender;
- (IBAction)goBackToForm:(id)sender;
- (IBAction)saveEvent:(id)sender;

@property (strong, nonatomic) IBOutlet CreateEventView *createEventView;
@property (weak, nonatomic) IBOutlet UIImageView *locationImage;

@end