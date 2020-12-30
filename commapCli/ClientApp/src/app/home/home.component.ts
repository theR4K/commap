import { Component, OnInit } from '@angular/core';
import { Meet } from '../types/meet';
import { MeetsService } from '../services/meets.service'
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  lat = 53.678418;
  lng = 30.809007;
  zoom: number = 8;
  bounds: google.maps.LatLngBounds;
  meets: Meet[];

  constructor(private meetsService: MeetsService) { }

  time: number = Date.now();

  ngOnInit() {
    setInterval(() => {
      this.time = Date.now();
    }, 1000);
  }

  /*calcTime(d: Date): string{
    var tmp: number = d.getTime();// - this.time;
    if (tmp > 0)
      return 'start after: ' + (new DatePipe('en').transform(tmp, 'M/d/yy, h:mm a'));

    return "none"+tmp;
  }*/

  clickedMarker(ind: number) {
  }

  mapClicked($event: MouseEvent) {
    
  }

  onMapReady(map: google.maps.Map) {
    map.setOptions({
      zoomControl: true,
      zoomControlOptions: {
        position: google.maps.ControlPosition.RIGHT_BOTTOM,
        style: google.maps.ZoomControlStyle.DEFAULT
      }
    });
    this.bounds = map.getBounds();
    this.updateMeets();
  }

  boundsChange(bounds: google.maps.LatLngBounds) {
    this.bounds = bounds;
    
  }

  zoomChange(args: any) {
    this.updateMeets();
  }

  updateMeets() {
    this.meetsService.getMeets().subscribe(
      m => {
        this.meets = m;
      }
    );
  }
}
