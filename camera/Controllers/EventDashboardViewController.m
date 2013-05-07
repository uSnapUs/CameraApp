//
// Created by Owen Evans on 26/04/13.
// Copyright (c) 2013 Owen Evans. All rights reserved.
//
// To change the template use AppCode | Preferences | File Templates.
//

#import <AFNetworking/UIImageView+AFNetworking.h>
#import <AssetsLibrary/AssetsLibrary.h>
#import "EventDashboardViewController.h"
#import "Event.h"
#import "Photo.h"
#import "ALAssetsLibrary+CustomPhotoAlbum.h"
#import "Application.h"
#import "User.h"
#import "Underscore.h"
#import "DeviceRegistration.h"


@implementation EventDashboardViewController {


@private
    Event *_event;
    NSArray *_photos;
    NSTimer *timer;
    LoginViewController *loginController;
}

- (void)viewDidLoad {

}

- (void)eventLoaded:(NSNotification *)notification {
    if (notification && [notification userInfo] && [[notification userInfo] objectForKey:@"event"]) {
        Event *event = [[notification userInfo] objectForKey:@"event"];
        if (event && [event isEqual:event]) {
            [self setEvent:event];
            [[self streamView] reloadData];
        }
    }
}

- (void)eventUpdated:(NSNotification *)notification {
    if (notification && [notification userInfo] && [[notification userInfo] objectForKey:@"event"]) {
        Event *event = [[notification userInfo] objectForKey:@"event"];
        if (event && [event isEqual:event]) {
            [[Application sharedInstance] lookupEventByCode:[event code]];
        }
    }
}

- (void)setEvent:(Event *)event {
    _event = event;
    NSSortDescriptor *sortDescriptor;
    sortDescriptor = [[NSSortDescriptor alloc] initWithKey:@"creationTime" ascending:NO];
    _photos = [[event photos] sortedArrayUsingDescriptors:@[sortDescriptor]];
    [[self eventTitleLabel] setText:event.name];
}

- (Event *)event {
    return _event;
}

- (IBAction)goToMainMenu:(id)sender {

    [self dismissViewControllerAnimated:YES completion:nil];
}


- (void)loginViewController:(LoginViewController *)controller didClose:(BOOL)userInitiated {
    CGRect bounds = [[self view] bounds];
    CGRectMake(0, bounds.size.height, bounds.size.width, bounds.size.height);
    [UIView animateWithDuration:0.5 animations:^{
        [[loginController view] setCenter:[[self view] center]];
    }                completion:^(BOOL finished) {
        [[loginController view] removeFromSuperview];
        loginController = nil;
    }];
}

- (IBAction)showPickerView:(id)sender {
    if ([[Application sharedInstance] isAuthenticated]) {
        [[NSNotificationCenter defaultCenter] removeObserver:self name:kUserLoggedIn object:nil];
        if (loginController) {
            [self loginViewController:loginController didClose:YES];
        }
        DLCImagePickerController *picker = [[DLCImagePickerController alloc] init];
        [picker setDelegate:self];
        [self presentViewController:picker animated:YES completion:nil];
    }
    else {
        if (!loginController) {
            loginController = [[LoginViewController alloc] initWithNibName:@"LoginView" bundle:nil];
            loginController.delegate = self;
        }
        [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(showPickerView:) name:kUserLoggedIn object:nil];
        DDLogVerbose(@"login controler %@", [loginController class]);
        CGRect bounds = [[self view] bounds];
        [[loginController view] setFrame:CGRectMake(0, bounds.size.height, bounds.size.width, bounds.size.height)];
        [[self view] addSubview:[loginController view]];
        [[self view] bringSubviewToFront:[loginController view]];
        [UIView animateWithDuration:0.5 animations:^{
            [[loginController view] setCenter:[[self view] center]];
        }                completion:^(BOOL finished) {

        }];
    }

}

- (void)viewWillAppear:(BOOL)animated {
    [super viewWillAppear:animated];
    [[UIApplication sharedApplication] setStatusBarHidden:NO withAnimation:UIStatusBarAnimationSlide];
    if ([self event]) {
        [[self eventTitleLabel] setText:[[self event] name]];
    }
}

- (void)viewDidAppear:(BOOL)animated {
    [super viewDidAppear:animated];
    [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(eventUpdated:) name:kEventUpdated object:nil];
    [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(eventLoaded:) name:kEventFoundNotification object:nil];
    timer = [NSTimer scheduledTimerWithTimeInterval:30.0
                                             target:self
                                           selector:@selector(updateEvent:)
                                           userInfo:nil
                                            repeats:YES];


}

- (void)updateEvent:(id)currentTimer {
    DDLogVerbose(@"updating event");
    [[Application sharedInstance] lookupEventByCode:[[self event] code]];

}

- (void)imagePickerController:(DLCImagePickerController *)picker didFinishPickingMediaWithInfo:(NSDictionary *)info {
    [self dismissViewControllerAnimated:YES completion:^{
    }];
    if (info) {
        ALAssetsLibrary *library = [[ALAssetsLibrary alloc] init];
        NSData *data = [info objectForKey:@"data"];
        [library saveImage:[UIImage imageWithData:data] toAlbum:@"uSnap.us" completionBlock:^(NSURL *assetURL, NSError *error) {
            if (error) {
                DDLogError(@"unable to save image: %@", error);
            }
            else {
                DDLogVerbose(@"saved image to %@", [assetURL absoluteString]);
            }
        }     failureBlock:^(NSError *error) {
            DDLogError(@"unable to save image: %@", error);
        }];
        dispatch_async(dispatch_get_global_queue(DISPATCH_QUEUE_PRIORITY_BACKGROUND, (unsigned long) NULL), ^{
            [[Application sharedInstance] uploadPhoto:data ToEvent:[self event]];
        });


    }

}


- (void)imagePickerControllerDidCancel:(DLCImagePickerController *)picker {
    [self dismissViewControllerAnimated:YES completion:^{
    }];

}

- (void)viewDidDisappear:(BOOL)animated {
    if (timer) {
        [timer invalidate];
    }
    [[NSNotificationCenter defaultCenter] removeObserver:self];
}

- (void)viewDidUnload {
    [self setEventTitleLabel:nil];
    [super viewDidUnload];
}

- (int)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section {
    return [[[self event] photos] count];
}

- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath {

    if (!_photos || [_photos count] < indexPath.row) {
        return nil;
    }
    Photo *photo = [_photos objectAtIndex:(NSUInteger) indexPath.row];
    PhotoCellView *cell = [tableView dequeueReusableCellWithIdentifier:@"photoCell"];
    if (!cell) {
        NSArray *nib = [[NSBundle mainBundle] loadNibNamed:@"PhotoCellView" owner:self options:nil];
        cell = (PhotoCellView *) [nib objectAtIndex:0];
        [cell setDelegate:self];
    }
    [cell setPhoto:photo];
    DDLogVerbose(@"setting url to %@", [[photo thumbnailURL] absoluteString]);
    [[cell thumbnailImage] setImageWithURL:[photo thumbnailURL]];
    if ([photo postedBy]) {
        [[cell photoLabel] setText:[[photo postedBy] name]];
    }
    else {
        [[cell photoLabel] setText:@"anon"];
    }

    [cell setSelectionStyle:UITableViewCellSelectionStyleNone];
    return cell;

}

- (void)cellImageWasTapped:(PhotoCellView *)cell {

}

- (void)cellLikeWasTapped:(PhotoCellView *)cell {
    if ([[Application sharedInstance] isAuthenticated]) {
        DeviceRegistration *device = [[Application sharedInstance] currentDevice];
        if (!Underscore.find(cell.photo.likedBy, ^BOOL(NSString *userId) {
            if (device.user) {
                return [userId isEqualToString:device.user.serverId];
            }
            return false;
        })) {
            [[Application sharedInstance] registerLikeForPhoto:[cell photo] inEvent:[self event]];
        }
        else{
            [[Application sharedInstance] removeLikeForPhoto:[cell photo] inEvent:[self event]];
        }
    }
    else {
        if (!loginController) {
            loginController = [[LoginViewController alloc] initWithNibName:@"LoginView" bundle:nil];
            loginController.delegate = self;
        }
        DDLogVerbose(@"login controler %@", [loginController class]);
        CGRect bounds = [[self view] bounds];
        [[loginController view] setFrame:CGRectMake(0, bounds.size.height, bounds.size.width, bounds.size.height)];
        [[self view] addSubview:[loginController view]];
        [[self view] bringSubviewToFront:[loginController view]];
        [UIView animateWithDuration:0.5 animations:^{
            [[loginController view] setCenter:[[self view] center]];
        }                completion:^(BOOL finished) {

        }];
    }

}

@end