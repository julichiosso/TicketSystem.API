import { defineStore } from 'pinia';
import axios from 'axios';
import { API_URL } from './auth';

const statusMap = {
  0: 'Pendiente',
  1: 'EnProceso',
  2: 'Resuelto',
  3: 'Cerrado',
  4: 'EsperandoUsuario'
};

function safeArray(value) {
  return Array.isArray(value) ? value : [];
}

function normalizeComment(comment) {
  if (!comment) return null;
  return {
    id:       comment.id       ?? comment.Id,
    ticketId: comment.ticketId ?? comment.TicketId,
    autorId:  comment.autorId  ?? comment.AutorId,
    autor:    comment.autor    ?? comment.Autor    ?? 'Sistema',
    autorRol: comment.autorRol ?? comment.rol      ?? comment.Rol ?? null,
    mensaje:  comment.mensaje  ?? comment.Mensaje  ?? '',
    interno:  comment.interno  ?? comment.Interno  ?? false,
    fecha:    comment.fecha    ?? comment.Fecha    ?? new Date().toISOString()
  };
}

export const useTicketsStore = defineStore('tickets', {
  state: () => ({
    // Lista principal (admin/operador con paginación)
    tickets:      [],
    totalTickets: 0,
    currentPage:  1,
    pageSize:     25,
    totalPages:   1,

    // Filtros activos
    filtros: {
      titulo:     '',
      estado:     null,
      prioridad:  null,
      operadorId: null,
      usuarioId:  null,
      fechaDesde: null,
      fechaHasta: null,
      sortBy:     'fechacreacion',
      descending: true,
    },

    selectedTicket:   null,
    comments:         [],
    loading:          false,
    loadingComments:  false,
  }),

  actions: {
    // ── Admin: trae tickets paginados con filtros ──────────────────────────
    async fetchAll(extraFilters = {}) {
      this.loading = true;
      try {
        const params = {
          Page:      this.currentPage,
          PageSize:  this.pageSize,
          SortBy:    this.filtros.sortBy,
          Descending: this.filtros.descending,
          ...this._filtrosActivos(),
          ...extraFilters,
        };

        // Limpiar nulls/vacíos
        Object.keys(params).forEach(k => {
          if (params[k] === null || params[k] === '' || params[k] === undefined)
            delete params[k];
        });

        const response = await axios.get(`${API_URL}/tickets`, { params });
        const result   = response.data?.data;

        this.tickets      = safeArray(result?.data ?? result);
        this.totalTickets = result?.totalRecords ?? this.tickets.length;
        this.totalPages   = Math.ceil(this.totalTickets / this.pageSize) || 1;

        return this.tickets;
      } finally {
        this.loading = false;
      }
    },

    // ── Usuario: sus propios tickets (sin paginación) ─────────────────────
    async fetchMine() {
      this.loading = true;
      try {
        const response = await axios.get(`${API_URL}/tickets/mis-tickets`);
        this.tickets = safeArray(response.data);
        return this.tickets;
      } finally {
        this.loading = false;
      }
    },

    // ── Cambiar página ────────────────────────────────────────────────────
    async goToPage(page) {
      if (page < 1 || page > this.totalPages) return;
      this.currentPage = page;
      await this.fetchAll();
    },

    // ── Cambiar pageSize ──────────────────────────────────────────────────
    async changePageSize(size) {
      this.pageSize    = size;
      this.currentPage = 1;
      await this.fetchAll();
    },

    // ── Aplicar filtros ───────────────────────────────────────────────────
    async applyFiltros(nuevosFiltros = {}) {
      this.filtros      = { ...this.filtros, ...nuevosFiltros };
      this.currentPage  = 1;
      await this.fetchAll();
    },

    // ── Limpiar filtros ───────────────────────────────────────────────────
    async clearFiltros() {
      this.filtros = {
        titulo:     '',
        estado:     null,
        prioridad:  null,
        operadorId: null,
        usuarioId:  null,
        fechaDesde: null,
        fechaHasta: null,
        sortBy:     'fechacreacion',
        descending: true,
      };
      this.currentPage = 1;
      await this.fetchAll();
    },

    // ── Cambiar orden ─────────────────────────────────────────────────────
    async sortBy(campo) {
      if (this.filtros.sortBy === campo) {
        this.filtros.descending = !this.filtros.descending;
      } else {
        this.filtros.sortBy    = campo;
        this.filtros.descending = true;
      }
      this.currentPage = 1;
      await this.fetchAll();
    },

    // ── Helpers ───────────────────────────────────────────────────────────
    _filtrosActivos() {
      return {
        Titulo:     this.filtros.titulo    || null,
        Estado:     this.filtros.estado,
        Prioridad:  this.filtros.prioridad,
        OperadorId: this.filtros.operadorId,
        UsuarioId:  this.filtros.usuarioId,
        FechaDesde: this.filtros.fechaDesde,
        FechaHasta: this.filtros.fechaHasta,
      };
    },

      // ── Estado / CRUD ─────────────────────────────────────────────────────
      async updateStatus(ticketId, newStatus) {
      await axios.patch(`${API_URL}/tickets/${ticketId}/estado`, newStatus, {
        headers: { 'Content-Type': 'application/json' }
      });
      const newState = statusMap[newStatus] ?? newStatus;
      const index = this.tickets.findIndex(t => t.id === ticketId);
      if (index !== -1) {
        this.tickets[index] = { ...this.tickets[index], estado: newState };
      }
      if (this.selectedTicket?.id === ticketId)
        this.selectedTicket = { ...this.selectedTicket, estado: newState };
    },

    async deleteTicket(id) {
      await axios.delete(`${API_URL}/tickets/${id}`);
      this.tickets      = this.tickets.filter(t => t.id !== id);
      this.totalTickets = Math.max(0, this.totalTickets - 1);
      this.totalPages   = Math.ceil(this.totalTickets / this.pageSize) || 1;
    },

    async createTicket(data) {
      const response = await axios.post(`${API_URL}/tickets`, data);
      return response.data;
    },

    async assignOperator(ticketId, operadorId) {
      await axios.put(`${API_URL}/tickets/${ticketId}/asignar`, {
        ticketId,
        operadorId: operadorId === '' ? null : operadorId
      });
      const ticket = this.tickets.find(t => t.id === ticketId);
      if (ticket) ticket.operadorAsignadoId = operadorId || null;
      if (this.selectedTicket?.id === ticketId)
        this.selectedTicket = { ...this.selectedTicket, operadorAsignadoId: operadorId || null };
    },

    // ── Comentarios ───────────────────────────────────────────────────────
    async fetchComments(ticketId) {
      this.loadingComments = true;
      try {
        const response = await axios.get(`${API_URL}/tickets/${ticketId}/comments`);
        const raw      = response.data?.data ?? response.data ?? [];
        this.comments  = safeArray(raw).map(normalizeComment).filter(Boolean);
        if (this.selectedTicket?.id === ticketId)
          this.selectedTicket = { ...this.selectedTicket, _comments: this.comments };
        return this.comments;
      } finally {
        this.loadingComments = false;
      }
    },

    async addComment(ticketId, payload) {
      const response = await axios.post(`${API_URL}/tickets/${ticketId}/comments`, {
        mensaje: payload.mensaje,
        interno: !!payload.interno
      });
      const saved   = normalizeComment(response.data?.data ?? response.data);
      this.comments = [...this.comments, saved];
      const ticket  = this.tickets.find(t => t.id === ticketId);
      if (ticket) ticket._comments = [...safeArray(ticket._comments), saved];
      if (this.selectedTicket?.id === ticketId)
        this.selectedTicket = { ...this.selectedTicket, _comments: [...safeArray(this.selectedTicket._comments), saved] };
      return saved;
    },

    selectTicket(ticket) {
      this.selectedTicket = ticket;
      this.comments       = safeArray(ticket?._comments);
    },

    clearSelection() {
      this.selectedTicket = null;
      this.comments       = [];
    },

    async fetchMetrics() {
      const response = await axios.get(`${API_URL}/metricas`);
      return response.data?.data;
    },

    async fetchAuditLogs() {
      const response = await axios.get(`${API_URL}/auditoria`);
      return response.data?.data;
    }
  },

  getters: {
    byStatus: (state) => (status) =>
      state.tickets.filter(t => t.estado === status),

    hayFiltrosActivos: (state) =>
      !!(state.filtros.titulo || state.filtros.estado !== null ||
         state.filtros.prioridad !== null || state.filtros.operadorId ||
         state.filtros.usuarioId || state.filtros.fechaDesde || state.filtros.fechaHasta),

    stats: (state) => ({
      total:      state.tickets.length,
      pending:    state.tickets.filter(t => t.estado === 'Pendiente'        || t.estado === 0).length,
      inProgress: state.tickets.filter(t => t.estado === 'EnProceso'        || t.estado === 1).length,
      resolved:   state.tickets.filter(t => t.estado === 'Resuelto'         || t.estado === 2).length,
      closed:     state.tickets.filter(t => t.estado === 'Cerrado'          || t.estado === 3).length,
      waiting:    state.tickets.filter(t => t.estado === 'EsperandoUsuario' || t.estado === 4).length,
      urgent:     state.tickets.filter(t => t.prioridad === 'Alta'          || t.prioridad === 2).length,
    })
  }
});