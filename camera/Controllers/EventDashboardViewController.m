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
#import "PhotoCellView.h"
#import "ALAssetsLibrary+CustomPhotoAlbum.h"
#import "Application.h"


@implementation EventDashboardViewController {


@private
    Event *_event;
    NSArray *_photos;
    NSTimer *timer;
}

-(void)viewDidLoad {

}

- (void)eventLoaded:(NSNotification *)notification {
    if(notification&&[notification userInfo]&&[[notification userInfo] objectForKey:@"event"])
    {
        Event *event =[[notification userInfo] objectForKey:@"event"];
        if(event&&[event isEqual:event])
        {
            [self setEvent:event];
            [[self streamView] reloadData];
        }
    }
}

- (void)eventUpdated:(NSNotification *)notification {
    if(notification&&[notification userInfo]&&[[notification userInfo] objectForKey:@"event"])
    {
        Event *event =[[notification userInfo] objectForKey:@"event"];
        if(event&&[event isEqual:event])
        {
            [[Application sharedInstance] lookupEventByCode:[event code]];
        }
    }
}

-(void)setEvent:(Event *)event {
    _event = event;
    NSSortDescriptor *sortDescriptor;
    sortDescriptor = [[NSSortDescriptor alloc] initWithKey:@"creationTime" ascending:NO];
    _photos = [[event photos] sortedArrayUsingDescriptors:@[sortDescriptor]];
    [[self eventTitleLabel] setText:event.name];
}
-(Event *)event{
    return _event;
}

- (IBAction)goToMainMenu:(id)sender {

    [self dismissViewControllerAnimated:YES completion:nil];
}

- (IBAction)showPickerview:(id)sender {
    DLCImagePickerController *picker = [[DLCImagePickerController alloc]init];
    [picker setDelegate:self];
    [self presentViewController:picker animated:YES completion:nil];
    
}

-(void)viewWillAppear:(BOOL)animated {
    [super viewWillAppear:animated];
    [[UIApplication sharedApplication] setStatusBarHidden:NO withAnimation:UIStatusBarAnimationSlide];
    if([self event])                                {
        [[self eventTitleLabel] setText:[[self event]name] ];
    }
}
-(void)viewDidAppear:(BOOL)animated {
    [super viewDidAppear:animated];
    [[NSNotificationCenter defaultCenter] addObserver:self  selector:@selector(eventUpdated:) name:kEventUpdated object:nil];
    [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(eventLoaded:) name:kEventFoundNotification object:nil];
    timer = [NSTimer scheduledTimerWithTimeInterval:30.0
                                     target:self
                                   selector:@selector(updateEvent:)
                                   userInfo:nil
                                    repeats:YES];


}

- (void)updateEvent:(id)timer {
    DDLogVerbose(@"updating event");
    [[Application sharedInstance] lookupEventByCode:[[self event]code] ];

}

-(void)imagePickerController:(DLCImagePickerController *)picker didFinishPickingMediaWithInfo:(NSDictionary *)info {
    [self dismissViewControllerAnimated:YES completion:^{}];
    if (info) {
        ALAssetsLibrary *library = [[ALAssetsLibrary alloc] init];
        NSData *data = [info objectForKey:@"data"];
        [library saveImage:[UIImage imageWithData:data]  toAlbum:@"uSnap.us" completionBlock:^(NSURL *assetURL, NSError *error) {
            if(error){
                DDLogError(@"unable to save image: %@", error);
            }
            else{
                DDLogVerbose(@"saved image to %@", [assetURL absoluteString]);
            }
        } failureBlock:^(NSError *error) {
            DDLogError(@"unable to save image: %@", error);
        }];
        dispatch_async(dispatch_get_global_queue(DISPATCH_QUEUE_PRIORITY_BACKGROUND, (unsigned long)NULL), ^{
           [[Application sharedInstance]uploadPhoto:data ToEvent:[self event]];
        });


    }

}



-(void)imagePickerControllerDidCancel:(DLCImagePickerController *)picker {
    [self dismissViewControllerAnimated:YES completion:^{}];

}

-(void)viewDidDisappear:(BOOL)animated {
    if(timer){
       [timer invalidate];
    }
    [[NSNotificationCenter defaultCenter] removeObserver:self];
}

- (void)viewDidUnload {
    [self setEventTitleLabel:nil];
    [super viewDidUnload];
}

-(int)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section {
            return [[[self event] photos] count];
}

- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath {

    if(!_photos||[_photos count]<indexPath.row){
        return nil;
    }
    Photo *photo = [_photos objectAtIndex:(NSUInteger)indexPath.row];
    PhotoCellView *cell = [tableView dequeueReusableCellWithIdentifier:@"photoCell"];
    if(!cell)
    {
        NSArray *nib = [[NSBundle mainBundle] loadNibNamed:@"PhotoCellView" owner:self options:nil];
        cell = (PhotoCellView *)[nib objectAtIndex:0];
    }
    DDLogVerbose(@"setting url to %@", [[photo thumbnailURL] absoluteString]);
    [[cell thumbnailImage] setImageWithURL:[photo thumbnailURL]];
    return cell;
    
}

@end