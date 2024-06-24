export class InitPokerRoomRequest {
  constructor(
    public roomName: string,
    public playerName: string,
    public roomType: string,
    public groupNames: string[] = []
  ) {}
}
export interface InitPokerRoomResponse {
  link: string;
}
