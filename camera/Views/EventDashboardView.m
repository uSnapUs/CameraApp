//
// Created by Owen Evans on 26/04/13.
// Copyright (c) 2013 Owen Evans. All rights reserved.
//
// To change the template use AppCode | Preferences | File Templates.
//



#import "EventDashboardView.h"
#import <QuartzCore/QuartzCore.h>
#import <CoreGraphics/CoreGraphics.h>


@implementation EventDashboardView {

}
-(void)awakeFromNib {
    [super awakeFromNib];
    [[self eventTitleLabel] setFont:[UIFont fontWithName:@"GelatoScript" size:30]];
    [[[self topBarView] layer] setShadowRadius:5];
    [[[self topBarView] layer] setShadowOffset:CGSizeMake(0, 2)];
    [[[self topBarView] layer] setShadowOpacity:0.5];
    [[self streamView] setTableHeaderView:[self eventTableHeader]];

}
-(void)layoutSubviews {
    [super layoutSubviews];
    DDLogVerbose(@"resetting header frame height");

    CGRect viewBounds = [self bounds];
    [[self topBarView] setFrame:CGRectMake(0, 0, viewBounds.size.width, 51)];
}
@end