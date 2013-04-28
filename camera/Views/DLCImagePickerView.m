//
// Created by Owen Evans on 27/04/13.
// Copyright (c) 2013 Owen Evans. All rights reserved.
//
// To change the template use AppCode | Preferences | File Templates.
//


#import "DLCImagePickerView.h"
#import <QuartzCore/QuartzCore.h>

@implementation DLCImagePickerView {

}
-(void)awakeFromNib {
    [self setClipsToBounds:YES];
    [[self layer] setCornerRadius:5];
    [[[self topBarView] layer] setShadowRadius:5];
    [[[self topBarView] layer] setShadowOffset:CGSizeMake(0, 2)];
    [[[self topBarView] layer] setShadowOpacity:0.5];
}
-(void)layoutSubviews {
    [[self layer] setCornerRadius:5];
}
@end