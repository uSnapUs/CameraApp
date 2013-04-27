//
// Created by Owen Evans on 25/04/13.
// Copyright (c) 2013 Owen Evans. All rights reserved.
//
// To change the template use AppCode | Preferences | File Templates.
//


#import "MainMenuView.h"
#import <QuartzCore/QuartzCore.h>
#import <CoreGraphics/CoreGraphics.h>

@implementation MainMenuView {

}

-(void)awakeFromNib{
    [super awakeFromNib];
    [self addSubview:[self mainMenuViewContainer]];
    [[self eventCodeField] setFont:[UIFont fontWithName:@"ProximaNova-Bold" size:24]];
    [[self eventCodeField] removeFromSuperview];
    [[self eventCodeView] addSubview:[self eventCodeField]];
    [[self eventCodeField] setHidden:NO];
    [[self eventCodeField] setAlpha:1];

}
-(void)layoutSubviews {

    [[self findViewContainer] bringSubviewToFront:[self findNearbyButton]];
    [super layoutSubviews];
    [[self layer] setCornerRadius:5];
    [[self mainMenuViewContainer] setClipsToBounds:YES];
    [[self mainMenuViewContainer] setFrame:[self bounds]];
    [self bringSubviewToFront:[self mainMenuViewContainer]];
    [[self findViewContainer] setFrame:[self bounds]];
    [[[self topBar] layer] setShadowOpacity:0.5];
    [[[self topBar] layer] setShadowRadius:5];
    [[[self topBar] layer] setShadowOffset:CGSizeMake(0, 2)];
    CGRect findNearbyButtonFrame = [[self findNearbyButton] frame];
    findNearbyButtonFrame.origin.y = [self bounds].size.height- 83;
    [[self findNearbyButton] setFrame:findNearbyButtonFrame];
    CGRect fieldBounds = [[self eventCodeView]bounds];
    fieldBounds.size.height = fieldBounds.size.height-15;
    fieldBounds.size.width = fieldBounds.size.width-10;
    fieldBounds.origin.y = fieldBounds.origin.y+5;
    fieldBounds.origin.x =fieldBounds.origin.x+5;
    [[self eventCodeField] setFrame:fieldBounds];

}

- (void)goToFindView:(id)sender {
    [UIView animateWithDuration:0.5 animations:^{
        [[self findButton] setAlpha:0];
        [[self createButton] setAlpha:0];
        [[self tagline] setAlpha:0];

    } completion:^(BOOL success){

        [[self findButton] setHidden:YES];
        [[self createButton] setHidden:YES];
        [[self tagline] setHidden:YES];
        [[self findButton] setAlpha:1];
        [[self createButton] setAlpha:1];
        [[self tagline] setAlpha:1];
        [[self backButton] setHidden:NO];
        [UIView animateWithDuration:0.5 animations:^{
            CGPoint backButtonCenter = [[self backButton] center];
            backButtonCenter.y = backButtonCenter.y+100;
            [[self backButton] setCenter:backButtonCenter];
            [[self backButton] setAlpha:1];
            CGPoint currentBackgroundCentre = [[self mainMenuViewContainer] center];
            currentBackgroundCentre.y = currentBackgroundCentre.y-100;
            [[self mainMenuViewContainer] setCenter:currentBackgroundCentre];
            CGPoint logoCentre = [[self logo] center];
            logoCentre.y = logoCentre.y+50;
            [[self logo] setCenter:logoCentre];
        } completion:^(BOOL secondSuccess){
            [[self nearbyEventMapView] setShowsUserLocation:YES];
            [[self eventCodeView] setHidden:NO];
            [UIView animateWithDuration:0.2 animations:^{
                [[self eventCodeView] setAlpha:1];

            }];
        }];

    }];
}


- (void)goToMapView:(id)sender {
    [[self eventCodeField] resignFirstResponder];
    [UIView animateWithDuration:0.5 animations:^{
        [[self backButton] setAlpha:0];
        [[self findNearbyButton] setAlpha:0];
        [[self eventCodeView] setAlpha:0];
        CGPoint currentBackgroundCentre = [[self mainMenuViewContainer] center];
        currentBackgroundCentre.y = 0-[self bounds].size.height;
        [[self mainMenuViewContainer] setCenter:currentBackgroundCentre];
    } completion:^(BOOL success){
        [[self eventCodeView] setHidden:YES];
        [[self findNearbyButton] setHidden:YES];
        [[self backButton] setHidden:YES];
        [[self findNearbyButton] setAlpha:1];
        [[self backButton] setAlpha:1];
    }];

}

- (void)goToMainMenu:(id)sender {
    [[self eventCodeField] resignFirstResponder];
    if([[self findNearbyButton] isHidden]){
        [self animateToFullView];
    }
    else{
        [UIView animateWithDuration:0.2 animations:^{
            [[self eventCodeView] setAlpha:0];
            [[self backButton] setAlpha:0];

        } completion:^(BOOL done){
            [[self eventCodeView] setHidden:YES];
            [[self backButton] setHidden:YES];
            [self animateToFullView];
        }];
    }

}

- (void)animateToFullView {
    CGPoint backButtonCenter = [[self backButton] center];
    backButtonCenter.y = backButtonCenter.y-100;
    [[self backButton] setCenter:backButtonCenter];
    [[self findButton] setAlpha:0];
    [[self createButton] setAlpha:0];
    [[self tagline] setAlpha:0];
    [[self findButton] setHidden:NO];
    [[self createButton] setHidden:NO];
    [[self tagline] setHidden:NO];
    [UIView animateWithDuration:0.5 animations:^{
        CGPoint currentBackgroundCentre = [self center];
        [[self mainMenuViewContainer] setCenter:currentBackgroundCentre];
        CGPoint logoCentre = [[self logo] center];
        logoCentre.y = logoCentre.y-50;
        [[self logo] setCenter:logoCentre];

    } completion:^(BOOL success){
        [UIView animateWithDuration:0.2 animations:^{
            [[self findButton] setAlpha:1];
            [[self createButton] setAlpha:1];
            [[self tagline] setAlpha:1];
        }completion:^(BOOL done){
                [[self findNearbyButton]setHidden:NO];
        }];
    }];
}
@end