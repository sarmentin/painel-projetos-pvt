import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PainelProjetos, StatusProjeto } from './projeto.model';

@Injectable({ providedIn: 'root' })
export class ProjetosService {
  private readonly http = inject(HttpClient);

  listar(status?: StatusProjeto): Observable<PainelProjetos> {
    let params = new HttpParams();
    if (status) {
      params = params.set('status', status);
    }
    return this.http.get<PainelProjetos>('/api/projetos', { params });
  }
}