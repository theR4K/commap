import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Meet } from '../types/meet';

@Injectable({
  providedIn: 'root'
})
export class MeetsService {
  server: string = 'https://localhost:5001';
  readUri: string = 'api/MeetRead';

  constructor(private http: HttpClient) { }

  getMeets(): Observable<Meet[]> {
    return this.http.get<Meet[]>(`${this.server}/${this.readUri}`);
  }

  /*
  getMovie(id: number): Observable<Meet> {
    return this.http.get<Meet>(`${this.uri}/${id}`);
  }

  createMovie(movie: Meet) {
    return this.http.post(this.uri, movie);
  }

  deleteMovie(id: number) {
    return this.http.delete(`${this.uri}/${id}`);
  }

  updateMovie(movie: Partial<Meet>): Observable<Meet> {
    return this.http.put<Meet>(`${this.uri}/${movie.id}`, movie);
  }
  */
}
