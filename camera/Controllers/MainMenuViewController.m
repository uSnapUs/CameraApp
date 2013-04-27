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
#import "WBErrorNoticeView.h"

@interface MainMenuViewController ()

@end

@implementation MainMenuViewController {
    NSArray *localEvents;
}

-(void)viewDidLoad {
    [[[self mainMenu] nearbyEventMapView] setDelegate:self];
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
    
    [[[self mainMenu] nearbyEventMapView] addAnnotations:localEvents];
}

- (void)textFieldDidEndEditing:(UITextField *)textField {
    if([[textField text] length]>0)
    {
        [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(eventSelected:) name:kEventFoundNotification object:nil];
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
    EventDashboardViewController *eventDashboardViewController = [[EventDashboardViewController alloc] initWithNibName:@"EventDashboardView" bundle:[NSBundle mainBundle]];
    eventDashboardViewController.event = event;
    [self presentViewController:eventDashboardViewController animated:YES completion:nil];
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
@end

