import { Component, ViewChild, NgZone, ElementRef } from '@angular/core';
import { ChatService } from '../shared/chat.service';
import { FormBuilder, Form, FormGroup, FormControl, Validators } from '@angular/forms';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent {
  public form: FormGroup;
  public defaultSelected = 0;
  public selection: number;
  @ViewChild('textArea') private myScrollContainer: ElementRef;

  public users: string[] = ['All', 'User 1', 'User 2'];
  isConnected = false;
  constructor(private chatService: ChatService, private formBuilder: FormBuilder) {
    this.form = formBuilder.group({
      username: new FormControl(''),
      receiver: new FormControl(''),
      message: new FormControl({ value: '', disabled: true }),
      messages: new FormControl({ value: 'Welcome to chat!', disabled: true })
    });
  }

  connect() {
    this.chatService.connect(this.form.controls['username'].value);
    this.chatService.messages.subscribe(msg => {
      this.form.controls['messages'].setValue(`${this.form.controls['messages'].value}\n ${msg}`);
      this.form.controls['message'].setValue('');
      this.scrollToBottom();
      this.chatService.getAllConnected(this.form.controls['username'].value).subscribe((u: string[]) => {
        this.users = [];
        this.users.push('All');
        u.forEach(usr => this.users.push(usr));
      });
      this.isConnected = true;
      this.form.controls['message'].enable();
    });
  }

  sendMsg() {
    if (this.form.controls['receiver'].value === '' || this.form.controls['receiver'].value === 'All') {
      this.chatService.sendMessageToAll(this.form.controls['message'].value, this.form.controls['username'].value);
    } else {
      this.chatService.sendMessageToOne(this.form.controls['message'].value,
        this.form.controls['username'].value, this.form.controls['receiver'].value);
    }
  }

  scrollToBottom(): void {
    try {
      this.myScrollContainer.nativeElement.scrollTop = this.myScrollContainer.nativeElement.scrollHeight;
    } catch (err) { }
  }
}
