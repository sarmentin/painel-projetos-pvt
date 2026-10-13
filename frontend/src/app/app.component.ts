import { Component, LOCALE_ID, OnInit, inject, signal } from '@angular/core';
import { DatePipe, DecimalPipe, formatNumber } from '@angular/common';
import { ProjetosService } from './projetos.service';
import { PainelProjetos, StatusProjeto } from './projeto.model';

// novo: cada botão de filtro tem um valor (enviado à API) e um rótulo (exibido)
interface OpcaoFiltro {
  valor: StatusProjeto | undefined;
  rotulo: string;
}

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [DatePipe, DecimalPipe],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  private readonly projetosService = inject(ProjetosService);
  private readonly locale = inject(LOCALE_ID);

  // novo
  readonly filtros: OpcaoFiltro[] = [
    { valor: undefined, rotulo: 'Todos' },
    { valor: 'Critico', rotulo: 'Críticos' },
    { valor: 'Atencao', rotulo: 'Em atenção' },
    { valor: 'Saudavel', rotulo: 'Saudáveis' }
  ];

  readonly rotulosStatus: Record<StatusProjeto, string> = {
    Critico: 'Crítico',
    Atencao: 'Atenção',
    Saudavel: 'Saudável'
  };

  // Estado da tela
  readonly painel = signal<PainelProjetos | null>(null);
  readonly filtroAtual = signal<StatusProjeto | undefined>(undefined); // novo
  readonly carregando = signal(true);
  readonly erro = signal(false);

  ngOnInit(): void {
    this.carregar();
  }

  // novo: guarda o filtro escolhido e busca de novo na API
  filtrar(status: StatusProjeto | undefined): void {
    this.filtroAtual.set(status);
    this.carregar();
  }

  // Limita a 100% para a barra não sair da trilha; o texto mostra o valor real
  larguraBarra(valor: number | null): number {
    return Math.min(valor ?? 0, 100);
  }

  percentual(valor: number | null): string {
    return valor === null ? 'não calculado' : formatNumber(valor, this.locale, '1.0-1') + '%';
  }

  private carregar(): void {
    this.carregando.set(true);
    this.erro.set(false);

    this.projetosService.listar(this.filtroAtual()).subscribe({ // novo: envia o filtro
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