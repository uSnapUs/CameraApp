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

    int offset;
}

-(void)awakeFromNib{
    [super awakeFromNib];
    [self addSubview:[self mainMenuViewContainer]];
    [[self eventCodeField] setFont:[UIFont fontWithName:@"ProximaNova-Bold" size:24]];
    [[self eventCodeField] removeFromSuperview];
    [[self eventCodeView] addSubview:[self eventCodeField]];
    [[self eventCodeField] setHidden:NO];
    [[self eventCodeField] setAlpha:1];
    [[[self topBar] layer] setShadowOpacity:0.5];
    [[[self topBar] layer] setShadowRadius:5];
    [[[self topBar] layer] setShadowOffset:CGSizeMake(0, 2)];
    [[[self tagline] layer] setShadowOpacity:0.4];
    [[[self tagline] layer] setShadowOffset:CGSizeMake(0, 4)];
    [[self tagline] setFont:[UIFont fontWithName:@"ProximaNova-Bold" size:18]];
}
-(void)layoutSubviews {

    [[self findViewContainer] bringSubviewToFront:[self findNearbyButton]];
    [super layoutSubviews];
    [[self layer] setCornerRadius:5];
    [[self mainMenuViewContainer] setClipsToBounds:YES];
    [[self mainMenuViewContainer] setFrame:[self bounds]];
    [self bringSubviewToFront:[self mainMenuViewContainer]];
    [[self findViewContainer] setFrame:[self bounds]];

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
    offset = 100;
    CGPoint finalFindCenter = [[self eventCodeView] center];
    CGPoint initialFindCenter = [[self eventCodeView] center];
    initialFindCenter.x =[[self mainMenuViewContainer] center].x+[self bounds].size.width;
    initialFindCenter.y = initialFindCenter.y;
    finalFindCenter.x = [[self mainMenuViewContainer] center].x;
    [[self eventCodeView] setCenter:initialFindCenter];
    [[self eventCodeView] setHidden:NO];
    [[self eventCodeView] setAlpha:1];
    
    [UIView animateWithDuration:0.5 animations:^{
     [self hideInitialMenu];

    } completion:^(BOOL success){

        [[self findButton] setHidden:YES];
        [[self createButton] setHidden:YES];
        [[self backButton] setHidden:NO];
        [UIView animateWithDuration:0.5 animations:^{
            CGPoint backButtonCenter = [[self backButton] center];
            backButtonCenter.y = backButtonCenter.y+offset;
            [[self backButton] setCenter:backButtonCenter];
            [[self backButton] setAlpha:1];
            CGPoint currentBackgroundCentre = [[self mainMenuViewContainer] center];
            currentBackgroundCentre.y = currentBackgroundCentre.y-offset;
            [[self mainMenuViewContainer] setCenter:currentBackgroundCentre];
            CGPoint logoCenter = [[self logo] center];
            logoCenter.y = logoCenter.y+offset-(offset/2);
            [[self logo] setCenter:logoCenter];
            [[self logo] setTransform:CGAffineTransformMakeScale(0.8,0.8)];
        } completion:^(BOOL completed){
            CGPoint taglineCenter = [[self tagline]center];
            taglineCenter.y = taglineCenter.y + (offset/2);
            [[self tagline] setCenter:taglineCenter];
            [[self tagline] setText:@"Type in an event code to begin,\n"
                    "or search for nearby events."];
            [UIView animateWithDuration:0.5 animations:^{
                [[self tagline] setAlpha:1];
            }];
        }];
        [UIView animateWithDuration:0.5 delay:0.2 options:UIViewAnimationOptionCurveEaseIn animations:^{
            [[self eventCodeView] setCenter:finalFindCenter];
        } completion:^(BOOL secondSuccess){
                [[self nearbyEventMapView] setShowsUserLocation:YES];
    }];

    }];
}

- (void)hideInitialMenu {
    CGPoint findButtonCenter = [[self findButton] center];
    findButtonCenter.x = [self center].x - [self frame].size.width;
    CGPoint createButtonCenter = [[self createButton] center];
    createButtonCenter.x = [self center].x - [self frame].size.width;
    NSLog(@"find button center %d", findButtonCenter);
    [[self findButton] setCenter:findButtonCenter];
    [[self createButton] setCenter:createButtonCenter];
    [[self tagline] setAlpha:0];
}
- (void)showInitialMenu {
    CGPoint findButtonCenter = [[self findButton] center];
    findButtonCenter.x = [self center].x;
    CGPoint createButtonCenter = [[self createButton] center];
    createButtonCenter.x = [self  center].x;
    NSLog(@"find button center %d", findButtonCenter);
    [[self findButton] setCenter:findButtonCenter];
    [[self createButton] setCenter:createButtonCenter];
    [[self tagline] setAlpha:1];
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
        CGPoint eventCodeCenter = [[self eventCodeView] center];
        eventCodeCenter.x = eventCodeCenter.x + [self bounds].size.width;
                
        [UIView animateWithDuration:0.2 animations:^{
            [[self tagline]setAlpha:0];
            [[self eventCodeView] setCenter:eventCodeCenter];
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

    backButtonCenter.y = backButtonCenter.y-offset;
    [[self backButton] setCenter:backButtonCenter];
    [[self tagline] setAlpha:0];
    [[self tagline] setText:@"Photo collaboration, made easy."];
    CGPoint taglineCenter = [[self tagline]center];
    taglineCenter.y = taglineCenter.y - (offset/2);
    [[self tagline] setCenter:taglineCenter];
    [[self findButton] setHidden:NO];
    [[self createButton] setHidden:NO];

    [UIView animateWithDuration:0.5 animations:^{
        CGPoint currentBackgroundCentre = [self center];
        [[self mainMenuViewContainer] setCenter:currentBackgroundCentre];
        CGPoint logoCenter = [[self logo] center];
        logoCenter.y = logoCenter.y-(offset/2);
        [[self logo] setCenter:logoCenter];
        [[self logo] setTransform:CGAffineTransformMakeScale(1,1)];

    } completion:^(BOOL success){
        offset = 0;
        [UIView animateWithDuration:0.2 animations:^{
          [self showInitialMenu];
        }completion:^(BOOL done){
                [[self findNearbyButton]setHidden:NO];
        }];
    }];
}


@end