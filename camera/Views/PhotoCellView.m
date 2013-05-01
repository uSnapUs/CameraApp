//
// Created by Owen Evans on 27/04/13.
// Copyright (c) 2013 Owen Evans. All rights reserved.
//
// To change the template use AppCode | Preferences | File Templates.
//


#import <CoreGraphics/CoreGraphics.h>
#import "PhotoCellView.h"
#import "Photo.h"


@implementation PhotoCellView {

@private
    Photo *_photo;
}

- (void)awakeFromNib {
    [[self photoLabel] setFont:[UIFont fontWithName:@"ProximaNova-Bold" size:22]];
    [[self photoLabel] setTextColor:[UIColor colorWithRed:53/(float)255 green:51/(float)255 blue:43/(float)255 alpha:1]];
}
@synthesize photo = _photo;

- (IBAction)imageWasTapped:(id)sender {
    DDLogVerbose(@"image was tapped: %@", [[[self photo] fullURL] absoluteString]);
}
- (BOOL)gestureRecognizer:(UIGestureRecognizer *)gestureRecognizer shouldReceiveTouch:(UITouch *)touch {
    CGPoint pointInView =  [touch locationInView:self];
    DDLogVerbose(@"point in view %d %d", pointInView.x, pointInView.y);
    if(CGRectContainsPoint([[self thumbnailImage]frame], pointInView))
        return YES;
    else
        return NO;
}
@end