import { Injectable } from '@angular/core';
import { Http, Response, RequestOptions, Headers } from '@angular/http';
import { Observable } from 'rxjs';

@Injectable()
export class HomeService {
    
    constructor(private http: Http) {
    
    }


    GetHomeMessage(): Observable<any[]> {
        return this.http.get(`api/default`)
            .map((res: Response) => res.json())
            .catch((error: any) => Observable.throw(error.json().error || 'Server error'))
    }
    
}