//
// Created by Owen Evans on 29/04/13.
// Copyright (c) 2013 Owen Evans. All rights reserved.
//
// To change the template use AppCode | Preferences | File Templates.
//


#import <CoreGraphics/CoreGraphics.h>
#import "CreateEventViewController.h"
#import "CreateEventView.h"
#import "MKMapView+ZoomLevel.h"
#import "MHPrettyDate.h"
#import "Event.h"
#import "Location.h"
#import "Application.h"


@implementation CreateEventViewController {

    BOOL private;
    CLLocationCoordinate2D location;
    NSDate *selectedDate;
    NSString *eventName;
    BOOL locationSet;
    Event *annotation;
}
- (void)viewDidUnload {
    [self setCreateEventView:nil];
    [self setLocationImage:nil];
    [self setMapView:nil];
    [super viewDidUnload];
}
- (IBAction)goToMainMenu:(id)sender {
    [[[self createEventView] eventNameField] resignFirstResponder];
    [self dismissViewControllerAnimated:YES completion:^{}];

}

- (IBAction)goToChooseLocation:(id)sender {
    [[[self createEventView] eventNameField] resignFirstResponder];
    [[self createEventView]showMapView];
}


- (IBAction)showEventDateChooser:(id)sender {

    [[[self createEventView] eventNameField] resignFirstResponder];
    [[[self createEventView] eventNameField] setEnabled:NO];
    CKCalendarView *calendarView = [[CKCalendarView alloc] init];
    CGRect calendarFrame = [calendarView frame];
    calendarFrame.origin.y = [[self view]bounds].size.height;
    [calendarView setFrame:calendarFrame];
    [[self view] addSubview:calendarView];
    if(selectedDate)
    {
        [calendarView setMonthShowing:selectedDate];
    }


    calendarFrame.origin.y = [[self view] bounds].size.height/2 - calendarFrame.size.height/2;

    [UIView animateWithDuration:0.5 animations:^{
        calendarView.frame = calendarFrame;
    } completion:^(BOOL complete){
        [[[self createEventView] eventNameField] setEnabled:YES];
    }];

    [calendarView setDelegate:self];
}

- (void)calendar:(CKCalendarView *)calendar didSelectDate:(NSDate *)date {
    selectedDate = date;
    [[[self createEventView] dateLabel] setText:[MHPrettyDate prettyDateFromDate:selectedDate withFormat:MHPrettyDateFormatNoTime]];
    [[[self createEventView] dateLabel] setTextColor:[UIColor blackColor]];
    CGRect calendarFrame = [calendar frame];
    calendarFrame.origin.y = [[self view]bounds].size.height;
    [UIView animateWithDuration:0.5 animations:^{
        calendar.frame = calendarFrame;
    } completion:^(BOOL complete){
        [calendar removeFromSuperview];
    }];

}
- (BOOL)calendar:(CKCalendarView *)calendar willSelectDate:(NSDate *)date {
    return [MHPrettyDate isFutureDate:date]||[MHPrettyDate isToday:date];
}
-(BOOL)calendar:(CKCalendarView *)calendar willChangeToMonth:(NSDate *)date {
    if([MHPrettyDate isFutureDate:date])
        return YES;
    else {
        NSDateComponents *dateCompenents = [[NSCalendar currentCalendar] components:NSDayCalendarUnit | NSMonthCalendarUnit | NSYearCalendarUnit fromDate:date];
        NSDateComponents *todayComponents = [[NSCalendar currentCalendar] components:NSDayCalendarUnit | NSMonthCalendarUnit | NSYearCalendarUnit fromDate:[NSDate date]];
        if(dateCompenents.month==todayComponents.month&&dateCompenents.year==todayComponents.year){
            return YES;
        }
        else{
            return NO;
        }
    }
}
-(void)calendar:(CKCalendarView *)calendar configureDateItem:(CKDateItem *)dateItem forDate:(NSDate *)date {
    if(selectedDate&& [date isEqualToDate:selectedDate])
    {
        dateItem.backgroundColor=[UIColor colorWithRed:0.06 green:0.729 blue:0.73 alpha:1];
    }
    else if([MHPrettyDate isPastDate:date])
    {
        [dateItem setTextColor:[UIColor grayColor]];
        [dateItem setBackgroundColor:[UIColor lightGrayColor]];
    }
}
- (IBAction)togglePublic:(id)sender {
    if(private)
    {
        private = NO;
        [[self createEventView] setToggleToPublic];
    }
    else
    {
        private = YES;
        [[self createEventView] setToggleToPrivate];
    }
}

- (IBAction)goBackToForm:(id)sender {
    [[self createEventView]hideMapView];
}

- (IBAction)saveEvent:(id)sender {
    if(eventName&&selectedDate&&(locationSet))
    {
        Event *event = [[Event alloc] init];
        event.name = eventName;
        event.location = [[Location alloc]init];

        event.location.coordinates = @[
                [NSNumber numberWithDouble:location.longitude],[NSNumber numberWithDouble:location.latitude]];
        event.start_date = selectedDate;

        NSDateComponents *components = [[NSDateComponents alloc] init];
        [components setDay:1];

        NSCalendar *gregorian = [[NSCalendar alloc] initWithCalendarIdentifier:NSGregorianCalendar];

        event.end_date = [gregorian dateByAddingComponents:components toDate:selectedDate options:0];

        event.is_public = !private;
        event.address = @"Some address";

        [[Application sharedInstance]saveEvent:event];
        [self dismissViewControllerAnimated:YES completion:nil];

    }
}

- (IBAction)dropPin:(id)sender {
    if(annotation)
    {
        [[self mapView] removeAnnotation:annotation];
    }
    if(locationSet){
        annotation = [[Event alloc]init];
                                         annotation.name=@"Drag To Location";
        annotation.location = [[Location alloc]init];
        annotation.location.coordinates=@[
                [NSNumber numberWithDouble:location.longitude],
                [NSNumber numberWithDouble:location.latitude]
        ];

        [[self mapView] addAnnotation:annotation];
    }
}

- (void)mapView:(MKMapView *)mapView didUpdateUserLocation:(MKUserLocation *)userLocation {
    if(!locationSet){
        [mapView setCenterCoordinate:userLocation.coordinate zoomLevel:14 animated:YES];
        location = userLocation.coordinate;
        locationSet = YES;
        [[[self createEventView] locationLabel] setText:@"My Location"];
        [[[self createEventView] locationLabel] setTextColor:[UIColor blackColor]];
        [[self locationImage] setImage:[UIImage imageNamed:@"location_active.png"]];
    }
}
-(MKAnnotationView *)mapView:(MKMapView *)mapView viewForAnnotation:(id <MKAnnotation>)newAnnotation {
    if(![newAnnotation isKindOfClass:[Event class]]){
        return nil;
    }
    else{
        MKAnnotationView *annotationView = [mapView dequeueReusableAnnotationViewWithIdentifier:@"eventAnnotationView"];
        if(!annotationView)
        {
            annotationView = [[MKPinAnnotationView alloc] initWithAnnotation:newAnnotation reuseIdentifier:@"eventAnnotationView"];
            [annotationView setDraggable:YES];
            [annotationView setCanShowCallout:YES];

            UIButton *calloutButton = [UIButton buttonWithType:UIButtonTypeContactAdd];

            [annotationView setRightCalloutAccessoryView:calloutButton];

            
        }
        [annotationView prepareForReuse];
        [annotationView setAnnotation:newAnnotation];
         return annotationView;
        
    }
}

- (void)mapView:(MKMapView *)mapView annotationView:(MKAnnotationView *)view calloutAccessoryControlTapped:(UIControl *)control
{
    if ([view isKindOfClass:[MKPinAnnotationView class]]) {
        location = view.annotation.coordinate;
        locationSet = YES;
        [[[self createEventView] locationLabel] setText:@"Custom Location"];
        [[[self createEventView] locationLabel] setTextColor:[UIColor blackColor]];
        [[self locationImage] setImage:[UIImage imageNamed:@"location_unactive.png"]];
        [self goBackToForm:nil];
    }
}

- (void)textFieldDidEndEditing:(UITextField *)textField {
    eventName = textField.text;
}
- (BOOL)textFieldShouldReturn:(UITextField *)textField {
    [textField resignFirstResponder];
    return YES;
}
@end