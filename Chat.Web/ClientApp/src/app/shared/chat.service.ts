import { Injectable } from '@angular/core';
import { WebsocketService } from './websocket.service';
import { Subject } from 'rxjs';
import { map } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';

const CHAT_URL = 'ws://localhost:1717/notification';

export interface Message {
  author: string;
  message: string;
}

@Injectable()
export class ChatService {
  public messages: Subject<any>;

  constructor(private wsService: WebsocketService, private httpClient: HttpClient) {
  }

  connect(username: string) {
    this.messages = <Subject<any>>this.wsService.connect(`${CHAT_URL}?username=${username}`).pipe(map(
      (response: MessageEvent): any => {
        return response.data;
      }
    ));
  }

  getAllConnected(sender: string) {
    return this.httpClient.get(`http://localhost:1717/api/chat/getAllConnected?username=${sender}`);
  }

  sendMessageToAll(message: string, sender: string) {
    this.httpClient.get(`http://localhost:1717/api/chat?message=${message}&username=${sender}`).subscribe(() => true);
  }

  sendMessageToOne(message: string, sender: string, receiver: string) {
    this.httpClient.get(`http://localhost:1717/api/chat/receiver?message=${message}&username=${sender}&receiver=${receiver}`)
      .subscribe(() => true);
  }
}

