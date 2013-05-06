//
// Created by Owen Evans on 27/04/13.
// Copyright (c) 2013 Owen Evans. All rights reserved.
//
// To change the template use AppCode | Preferences | File Templates.
//


#import <Foundation/Foundation.h>
#import <ActiveTouch/ATModel.h>

@class User;


@interface Photo : ATModel
    @property (nonatomic,copy) NSString *rootUrl;
    @property (nonatomic,copy) NSNumber *likes;
    @property (nonatomic,copy) NSString *id;
    @property (nonatomic,copy) NSString *thumbnailPath;
    @property (nonatomic,copy) NSString *fullPath;
    @property (nonatomic,copy) NSDate *createdAt;
    @property (nonatomic,copy) NSDate *creationTime;
    @property (nonatomic,copy) User *postedBy;

- (NSURL *)fullURL;

- (NSURL *)thumbnailURL;
@end