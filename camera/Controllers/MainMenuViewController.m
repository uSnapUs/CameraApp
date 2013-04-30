//
//  MainMenuViewController.m
//  camera
//
//  Created by Owen Evans on 04/25/13.
//  Copyright (c) 2013 Owen Evans. All rights reserved.
//

#import "MainMenuViewController.h"
#import "MainMenuView.h"
#import "MKMapView+ZoomLevel.h"
#import "Application.h"
#import "Event.h"
#import "EventDashboardViewController.h"
#import "CreateEventViewController.h"

@interface MainMenuViewController ()

@end

@implementation MainMenuViewController {
    NSArray *localEvents;
    LoginViewController *loginController;
}

-(void)viewDidLoad {
    [[[self mainMenu] nearbyEventMapView] setDelegate:self];

}
-(void)viewDidAppear:(BOOL)animated {
    [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(eventSelected:) name:kEventFoundNotification object:nil];
}
-(void)viewDidDisappear:(BOOL)animated {
    [[NSNotificationCenter defaultCenter] removeObserver:self];
    [[[self mainMenu] nearbyEventMapView] setShowsUserLocation:NO];
    [[self mainMenu] goToMainMenu:nil];
    [[[self mainMenu] eventCodeField] setText:nil];
}

-(void)viewWillAppear:(BOOL)animated {
    [super viewWillAppear:animated];
    [[UIApplication sharedApplication] setStatusBarHidden:YES withAnimation:UIStatusBarAnimationSlide];
}
- (void)viewDidUnload {
    [self setMainMenu:nil];
    [super viewDidUnload];
}

-(void)mapView:(MKMapView *)mapView didUpdateUserLocation:(MKUserLocation *)userLocation {
    [mapView setCenterCoordinate:userLocation.coordinate zoomLevel:14 animated:YES];
    [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(addEventsToMap:) name:kEventsForLocationLookedUp object:nil];
    [[Application sharedInstance] loadEventsCloseTo:[userLocation coordinate]];
}

- (void)addEventsToMap:(NSNotification *)notification {
    DDLogVerbose(@"got events for map %@", [notification.userInfo objectForKey:@"events"]);
    localEvents = [notification.userInfo objectForKey:@"events"];

    NSMutableArray *annotationsToRemove = [[NSMutableArray alloc]init];
    NSArray *allExistingAnnotations = [[[self mainMenu] nearbyEventMapView] annotations];
    for (id annotation in allExistingAnnotations) {
        if([annotation isMemberOfClass:[Event class]]){
            [annotationsToRemove addObject:annotation];
        }
    }
    [[[self mainMenu] nearbyEventMapView] removeAnnotations:annotationsToRemove];
    [[[self mainMenu] nearbyEventMapView] addAnnotations:localEvents];
}

- (void)textFieldDidEndEditing:(UITextField *)textField {
    if([[textField text] length]>0)
    {

        [[Application sharedInstance]lookupEventByCode:[textField text]];

    }
}
-(BOOL)textFieldShouldReturn:(UITextField *)textField {
    [textField resignFirstResponder];
    return YES;

}


-(void)eventSelected:(NSNotification*)notification{

    if([notification userInfo]&& [[notification userInfo] objectForKey:@"event"]){
      [self goToEventDashboardForEvent:[[notification userInfo] objectForKey:@"event"]];
    }
    else{
        DDLogVerbose(@"object was not an event");
    }

}
-(void)goToEventDashboardForEvent:(Event *)event{

    EventDashboardViewController *eventDashboardViewController = [[EventDashboardViewController alloc] initWithNibName:@"EventDashboardView" bundle:nil];
    eventDashboardViewController.event = event;
    [self presentViewController:eventDashboardViewController animated:YES completion:^{

    }];

}
- (void)mapView:(MKMapView *)mapView didAddAnnotationViews:(NSArray *)views {

}
- (MKAnnotationView *)mapView:(MKMapView *)mapView viewForAnnotation:(id <MKAnnotation>)annotation {
    if(![[annotation class] isEqual:[Event class]])
        return nil;
    MKPinAnnotationView *view = (MKPinAnnotationView *) [mapView dequeueReusableAnnotationViewWithIdentifier:@"event_annotation_view"];
    if(view)
    {
        [view prepareForReuse];
    }
    else{

        view = [[MKPinAnnotationView alloc] initWithAnnotation:annotation reuseIdentifier:@"event_annotation_view"];
    }
    [view setPinColor:MKPinAnnotationColorGreen];
    [view setAnimatesDrop:YES];
    [view setCanShowCallout:YES];
    UIButton *calloutButton = [UIButton buttonWithType:UIButtonTypeDetailDisclosure];
    [calloutButton addTarget:self action:@selector(mapAnnotationSelected:) forControlEvents:UIControlEventTouchUpInside];
    [view setRightCalloutAccessoryView:calloutButton];
    [view setAnnotation:annotation];

    
    return view;
}

- (void)mapAnnotationSelected:(id)sender {
    Event *event = [[[[self mainMenu] nearbyEventMapView] selectedAnnotations] objectAtIndex:0];
    if(event){
        [self goToEventDashboardForEvent:event];
    }
}

-(void)dealloc {
    [[NSNotificationCenter defaultCenter] removeObserver:self];
}
- (void)loginViewController:(LoginViewController *)controller didClose:(BOOL)userInitiated {
    CGRect bounds = [[self view]bounds];
    CGRectMake(0, bounds.size.height, bounds.size.width, bounds.size.height);
    [UIView animateWithDuration:0.5 animations:^{
        [[loginController view] setCenter:[[self view]center]];
    } completion:^(BOOL finished) {
         [[loginController view] removeFromSuperview];
        loginController = nil;
    }];
}
- (IBAction)goToCreateView:(id)sender {
    if([[Application sharedInstance]isAuthenticated]){
        [[NSNotificationCenter defaultCenter] removeObserver:self name:kUserLoggedIn object:nil];
        if(loginController)
        {
            [self loginViewController:loginController didClose:YES];
        }
        CreateEventViewController *createEventViewController = [[CreateEventViewController alloc] initWithNibName:@"CreateEventView" bundle:nil];
        [self presentViewController:createEventViewController animated:YES completion:nil];
    }
    else{
        if(!loginController){
            loginController = [[LoginViewController alloc] initWithNibName:@"LoginView" bundle:nil];
            loginController.delegate=self;
        }
        [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(goToCreateView:) name:kUserLoggedIn object:nil];
        DDLogVerbose(@"login controler %@", [loginController class]);
        CGRect bounds = [[self view]bounds];
        [[loginController view]setFrame: CGRectMake(0, bounds.size.height, bounds.size.width, bounds.size.height)];
        [[[self mainMenu] mainMenuViewContainer] addSubview:[loginController view] ];
        [[[self mainMenu] mainMenuViewContainer] bringSubviewToFront:[loginController view]];
        [UIView animateWithDuration:0.5 animations:^{
            [[loginController view] setCenter:[[self view]center]];
        } completion:^(BOOL finished) {

        }];
    }




}
@end

