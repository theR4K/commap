export interface geoPoint {
  type: string,
  coordinates: number[]
}

export enum MeetStates {
//  deleted = -1,
//  ended = 0,
  planned = 1,
  active = 2
}

export interface Meet {
  id: string,
  name: string,
  location: geoPoint,
  subject: string,
  startTime: Date,
  endTime: Date,
  maxMembers: number,
  state : MeetStates
}
