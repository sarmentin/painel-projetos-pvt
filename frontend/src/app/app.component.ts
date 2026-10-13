import { Component, OnInit, inject, signal } from '@angular/core';
import { ProjetosService } from './projetos.service';
import { PainelProjetos } from './projeto.model';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  private readonly projetosService = inject(ProjetosService);

  // Estado da tela
  readonly painel = signal<PainelProjetos | null>(null);
  readonly carregando = signal(true);
  readonly erro = signal(false);

  ngOnInit(): void {
    this.carregar();
  }

  private carregar(): void {
    this.carregando.set(true);
    this.erro.set(false);

    this.projetosService.listar().subscribe({
      next: dados => {
        this.painel.set(dados);
        this.carregando.set(false);
      },
      error: () => {
        this.erro.set(true);
        this.carregando.set(false);
      }
    });
  }
}