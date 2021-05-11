import { Router } from '@angular/router';

import { AuthenticationService } from '../services';

export function appInitializer(authenticationService: AuthenticationService) {
    return () => new Promise(resolve => {
        authenticationService.refreshToken().subscribe().add(resolve);
    });
}
