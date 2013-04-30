//
//  MainMenuViewController.h
//  camera
//
//  Created by Owen Evans on 04/25/13.
//  Copyright (c) 2013 Owen Evans. All rights reserved.
//



#import <MapKit/MapKit.h>
#import "LoginViewController.h"

@class MainMenuView;

@interface MainMenuViewController : UIViewController <MKMapViewDelegate,UITextFieldDelegate, LoginDelegate>

@property (strong, nonatomic) IBOutlet MainMenuView *mainMenu;

- (IBAction)goToCreateView:(id)sender;




@end
