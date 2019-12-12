#import "UnityAppController.h"

@interface OverrideUnityAppController : UnityAppController

- (BOOL)application:(UIApplication*)application openURL:(NSURL*)url sourceApplication:(NSString*)sourceApplication annotation:(id)annotation;

@end