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


@implementation CreateEventViewController {

    BOOL private;
    CLLocationCoordinate2D location;
    NSDate *selectedDate;
}
- (void)viewDidUnload {
    [self setCreateEventView:nil];
    [self setLocationImage:nil];
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

- (void)mapView:(MKMapView *)mapView didUpdateUserLocation:(MKUserLocation *)userLocation {
    [mapView setCenterCoordinate:userLocation.coordinate zoomLevel:14 animated:YES];
    location = userLocation.coordinate;
    [[[self createEventView] locationLabel] setText:@"My Location"];
    [[[self createEventView] locationLabel] setTextColor:[UIColor blackColor]];
    [[self locationImage] setImage:[UIImage imageNamed:@"location_active.png"]];
}
@end