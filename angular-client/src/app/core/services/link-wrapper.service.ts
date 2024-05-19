import { Injectable } from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';

@Injectable({
  providedIn: 'root',
})
export class LinkWrapperService {
  constructor(private sanitizer: DomSanitizer) {}
  replaceUrlsWithLinks(text: string): SafeHtml {
    const urlPattern =
      /((http|https|ftp):\/\/[\w?=&.\/-;#~%-]+(?![\w\s?&.\/;#~%"=-]*>))/g;
    const replacement = (match: string) =>
      `<a href="${match}" class='text-primary-blue-500 hover:text-primary-blue-300 underline' target="_blank">${match}</a>`;
    const safeHtml = this.sanitizer.bypassSecurityTrustHtml(
      text.replace(urlPattern, replacement)
    );
    return safeHtml;
  }
}
