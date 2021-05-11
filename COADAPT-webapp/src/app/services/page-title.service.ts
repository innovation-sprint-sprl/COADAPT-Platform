import {Injectable, OnDestroy} from '@angular/core';


@Injectable({
  providedIn: 'root',
})
export class PageTitleService implements OnDestroy{
  private title: string;

  public setTitle(pageTitle: string) {
    this.title = pageTitle;
  }

  public getTitle() {
    return this.title;
  }

  ngOnDestroy() {
    this.title = '';
  }

}
