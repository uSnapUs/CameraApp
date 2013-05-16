//
// Created by Owen Evans on 29/04/13.
// Copyright (c) 2013 Owen Evans. All rights reserved.
//
// To change the template use AppCode | Preferences | File Templates.
//


#import <Foundation/Foundation.h>


@interface CreateEventView : UIView

@property (weak, nonatomic) IBOutlet UILabel *dateLabel;
@property (weak, nonatomic) IBOutlet UITextField *eventNameField;
@property (weak, nonatomic) IBOutlet UILabel *locationLabel;
@property (weak, nonatomic) IBOutlet UIButton *toggleButton;
@property (weak, nonatomic) IBOutlet UIButton *mainBackButton;
@property (weak, nonatomic) IBOutlet UIImageView *eventNameBackground;
@property (weak, nonatomic) IBOutlet UIButton *locationFieldBackground;
@property (weak, nonatomic) IBOutlet UIImageView *logoView;

@property (strong, nonatomic) IBOutlet UIView *mapViewContainer;
@property (weak, nonatomic) IBOutlet UIButton *eventDateFieldBackground;
@property (weak, nonatomic) IBOutlet UIImageView *publicToggleBackground;
@property (weak, nonatomic) IBOutlet UIButton *createButton;
@property (weak, nonatomic) IBOutlet UIImageView *locationPositionImage;

-(void)setToggleToPublic;
-(void)setToggleToPrivate;

- (void)showMapView;

- (void)hideMapView;

- (void)animateIn;

- (void)animateOut:(void (^)())onCompleteBlock;
@end