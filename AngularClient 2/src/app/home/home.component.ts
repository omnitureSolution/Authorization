import { AuthService } from 'src/app/core/auth/auth.service';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent implements OnInit {
  constructor(private route: ActivatedRoute, private authService: AuthService) { }

  ngOnInit(): void {
    if (this.route.snapshot.queryParams.auto) {
      this.authService.login();
    }
  }
}
