//
// Created by Owen Evans on 30/04/13.
// Copyright (c) 2013 Owen Evans. All rights reserved.
//
// To change the template use AppCode | Preferences | File Templates.
//


#import "LoginViewController.h"
#import "Application.h"


@implementation LoginViewController {

}
@synthesize delegate;

- (IBAction)login:(id)sender {

    [[Application sharedInstance]login];
}


- (IBAction)closeView:(id)sender {
    if(delegate)
    {
        [delegate loginViewController:self didClose:YES];
    }

}
@end