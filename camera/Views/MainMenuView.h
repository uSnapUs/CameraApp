//
// Created by Owen Evans on 25/04/13.
// Copyright (c) 2013 Owen Evans. All rights reserved.
//
// To change the template use AppCode | Preferences | File Templates.
//


#import <Foundation/Foundation.h>
#import <MapKit/MapKit.h>


@interface MainMenuView : UIView
    @property (weak, nonatomic) IBOutlet UIButton *findButton;
    @property (weak, nonatomic) IBOutlet UIButton *createButton;
    @property (weak, nonatomic) IBOutlet UIButton *findNearbyButton;
    @property (weak, nonatomic) IBOutlet UIImageView *background;
    @property (weak, nonatomic) IBOutlet UIView *logo;
    @property (weak, nonatomic) IBOutlet UILabel *tagline;
    @property (weak, nonatomic) IBOutlet UIView *mainMenuViewContainer;
    @property (weak, nonatomic) IBOutlet UIView *findViewContainer;
    @property (weak, nonatomic) IBOutlet UIView *topBar;
    @property (weak, nonatomic) IBOutlet MKMapView *nearbyEventMapView;
    @property (weak, nonatomic) IBOutlet UIButton *backButton;
    @property (weak, nonatomic) IBOutlet UITextField *eventCodeField;
    @property (weak, nonatomic) IBOutlet UIImageView *eventCodeView;

-(IBAction) goToFindView:(id)sender;

- (IBAction)goToMapView:(id)sender;

- (IBAction)goToMainMenu:(id)sender;


- (void)transitionToCreateView:(void (^)())presentBlock;

- (void)animateIn;
@end