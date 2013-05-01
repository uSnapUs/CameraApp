//
// Created by Owen Evans on 29/04/13.
// Copyright (c) 2013 Owen Evans. All rights reserved.
//
// To change the template use AppCode | Preferences | File Templates.
//


#import "CreateEventView.h"
#import <QuartzCore/QuartzCore.h>
#import <CoreGraphics/CoreGraphics.h>

@implementation CreateEventView {



}
- (void)awakeFromNib {
    [[self dateLabel] setFont:[UIFont fontWithName:@"ProximaNova-Bold" size:24]];
    [[self eventNameField] setFont:[UIFont fontWithName:@"ProximaNova-Bold" size:24]];
    [[self locationLabel] setFont:[UIFont fontWithName:@"ProximaNova-Bold" size:24]];
    [self addSubview:[self mapViewContainer]];
    CGRect mapViewFrame = [[self mapViewContainer] frame];
    mapViewFrame.origin.x = [self bounds].size.width;
    [[self mapViewContainer] setFrame:mapViewFrame];

}

- (void)layoutSubviews {
    [super layoutSubviews];
    [[self layer] setCornerRadius:5];


}

- (void)setToggleToPublic {
    CGRect buttonFrame = [[self toggleButton] frame];
    buttonFrame.origin.x = 159;
    [[self toggleButton] setEnabled:NO];
    [UIView animateWithDuration:0.1 animations:^{
        [[self toggleButton] setFrame:buttonFrame];
    } completion:^(BOOL completed){
        [[self toggleButton] setEnabled:YES];
    }];

}

- (void)setToggleToPrivate {
    CGRect buttonFrame = [[self toggleButton] frame];
    buttonFrame.origin.x = 46;
    [[self toggleButton] setEnabled:NO];
    [UIView animateWithDuration:0.1 animations:^{
        [[self toggleButton] setFrame:buttonFrame];
    } completion:^(BOOL completed){
        [[self toggleButton] setEnabled:YES];
    }];
}

- (void)showMapView {
    CGRect mapContainerFrame = [[self mapViewContainer] frame];
    mapContainerFrame.origin.x=0;
    [UIView animateWithDuration:0.5 animations:^{
        [[self mapViewContainer] setFrame:mapContainerFrame];
    } completion:^(BOOL complete){
        [[self mainBackButton] setEnabled:NO];
    }];

}

- (void)hideMapView {
    CGRect mapContainerFrame = [[self mapViewContainer] frame];
    mapContainerFrame.origin.x=[self bounds].size.width;
    [UIView animateWithDuration:0.5 animations:^{
        [[self mapViewContainer] setFrame:mapContainerFrame];
    } completion:^(BOOL complete){
        [[self mainBackButton] setEnabled:YES];
    }];

}
@end