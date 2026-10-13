// Espelha os DTOs da API (PainelProjetosDto e ProjetoResumoDto)
export type StatusProjeto = 'Saudavel' | 'Atencao' | 'Critico';

export interface ProjetoResumo {
  id: number;
  nome: string;
  cliente: string;
  horasVendidas: number;
  horasPlanejadas: number;
  horasRealizadas: number;
  saldo: number;
  consumo: number | null;
  avancoFisico: number | null;
  status: StatusProjeto;
}

export interface PainelProjetos {
  sincronizadoEm: string | null;
  projetos: ProjetoResumo[];
}