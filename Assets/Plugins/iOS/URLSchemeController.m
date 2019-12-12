#import <UIKit/UIKit.h>
#import "UnityAppController.h"

@interface URLSchemeController : UnityAppController
{
}

- (BOOL) application:(UIApplication*)application didFinishLaunchingWithOptions:(NSDictionary*)launchOptions;

- (void) openURLAfterDelay:(NSURL*) url;

- (BOOL) application:(UIApplication *)application handleOpenURL:(NSURL *)url;

- (BOOL) application:(UIApplication *)application openURL:(NSURL *)url sourceApplication:(NSString *)sourceApplication annotation:(id)annotation;

@end
@implementation URLSchemeController

- (BOOL) application:(UIApplication*)application didFinishLaunchingWithOptions:(NSDictionary*)launchOptions
{
    [super application:application didFinishLaunchingWithOptions:launchOptions];
    
    if ([launchOptions objectForKey:UIApplicationLaunchOptionsURLKey]) {
        NSURL *url = [launchOptions objectForKey:UIApplicationLaunchOptionsURLKey];
        
        [self performSelector:@selector(openURLAfterDelay:) withObject:url afterDelay:4];
    }
    
    return YES;
}

- (void) openURLAfterDelay:(NSURL*) url
{
	if ([[url absoluteString] rangeOfString:@"miarqui"].location != NSNotFound){
		UnitySendMessage("URLSchemeHandler", "OnOpenWithUrl", [[url absoluteString] UTF8String]);
	}
}

- (BOOL) application:(UIApplication *)application handleOpenURL:(NSURL *)url
{
	if ([[url absoluteString] rangeOfString:@"miarqui"].location != NSNotFound){
	    UnitySendMessage("URLSchemeHandler", "OnOpenWithUrl", [[url absoluteString] UTF8String]);
	    return YES;
	}
	return NO;
}


- (BOOL) application:(UIApplication *)application openURL:(NSURL *)url sourceApplication:(NSString *)sourceApplication annotation:(id)annotation
{
	if ([[url absoluteString] rangeOfString:@"miarqui"].location != NSNotFound){
	    UnitySendMessage("URLSchemeHandler", "OnOpenWithUrl", [[url absoluteString] UTF8String]);
	    return YES;
	}
	return NO;
}
@end

IMPL_APP_CONTROLLER_SUBCLASS(URLSchemeController)
