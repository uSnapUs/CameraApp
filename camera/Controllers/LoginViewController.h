//
// Created by Owen Evans on 30/04/13.
// Copyright (c) 2013 Owen Evans. All rights reserved.
//
// To change the template use AppCode | Preferences | File Templates.
//


#import <Foundation/Foundation.h>

@class LoginViewController;


@protocol LoginDelegate <NSObject>
@optional
-(void)loginViewController:(LoginViewController*)controller didClose:(BOOL)userInitiated;
@end

@interface LoginViewController : UIViewController

@property(nonatomic, weak) id <LoginDelegate> delegate;

- (IBAction)login:(id)sender;
- (IBAction)closeView:(id)sender;

@end


