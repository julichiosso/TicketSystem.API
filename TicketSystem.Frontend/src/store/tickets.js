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

  // Soporta backend nuevo (PascalCase) y estructuras viejas (camelCase)
  return {
    id: comment.id ?? comment.Id,
    ticketId: comment.ticketId ?? comment.TicketId,
    autorId: comment.autorId ?? comment.AutorId,
    autor: comment.autor ?? comment.Autor ?? 'Sistema',
    autorRol: comment.autorRol ?? comment.rol ?? comment.Rol ?? null,
    mensaje: comment.mensaje ?? comment.Mensaje ?? '',
    interno: comment.interno ?? comment.Interno ?? false,
    fecha: comment.fecha ?? comment.Fecha ?? new Date().toISOString()
  };
}

export const useTicketsStore = defineStore('tickets', {
  state: () => ({
    tickets: [],
    selectedTicket: null,
    comments: [],
    loading: false,
    loadingComments: false,
  }),

  actions: {
    async fetchAll(filters = {}) {
      this.loading = true;
      try {
        const params = { Page: 1, PageSize: 100, ...filters };
        const response = await axios.get(`${API_URL}/tickets`, { params });
        const result = response.data?.data;
        this.tickets = (result && result.data) ? result.data : (Array.isArray(result) ? result : []);
        return this.tickets;
      } finally {
        this.loading = false;
      }
    },

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

    async updateStatus(ticketId, newStatus) {
      await axios.patch(`${API_URL}/tickets/${ticketId}/estado`, newStatus, {
        headers: { 'Content-Type': 'application/json' }
      });

      const newState = statusMap[newStatus] ?? newStatus;
      const ticket = this.tickets.find((item) => item.id === ticketId);
      if (ticket) ticket.estado = newState;

      if (this.selectedTicket?.id === ticketId) {
        this.selectedTicket = { ...this.selectedTicket, estado: newState };
      }
    },

    async deleteTicket(id) {
      await axios.delete(`${API_URL}/tickets/${id}`);
      this.tickets = this.tickets.filter((ticket) => ticket.id !== id);
    },

    async createTicket(data) {
      const response = await axios.post(`${API_URL}/tickets`, data);
      return response.data;
    },

    async fetchComments(ticketId) {
      this.loadingComments = true;
      try {
        const response = await axios.get(`${API_URL}/tickets/${ticketId}/comments`);
        const raw = response.data?.data ?? response.data ?? [];
        this.comments = safeArray(raw).map(normalizeComment).filter(Boolean);

        if (this.selectedTicket?.id === ticketId) {
          this.selectedTicket = { ...this.selectedTicket, _comments: this.comments };
        }

        return this.comments;
      } finally {
        this.loadingComments = false;
      }
    },


    async addComment(ticketId, payload) {
      const body = {
        mensaje: payload.mensaje,
        interno: !!payload.interno
      };

      const response = await axios.post(`${API_URL}/tickets/${ticketId}/comments`, body);
      const saved = normalizeComment(response.data?.data ?? response.data);

      this.comments = [...this.comments, saved];

      const ticket = this.tickets.find((item) => item.id === ticketId);
      if (ticket) {
        ticket._comments = [...safeArray(ticket._comments), saved];
      }

      if (this.selectedTicket?.id === ticketId) {
        this.selectedTicket = {
          ...this.selectedTicket,
          _comments: [...safeArray(this.selectedTicket._comments), saved]
        };
      }

      return saved;
    },

    async assignOperator(ticketId, operadorId) {
      await axios.put(`${API_URL}/tickets/${ticketId}/asignar`, {
        ticketId,
        operadorId: operadorId === '' ? null : operadorId
      });

      // Update local state without full refresh if possible
      const ticket = this.tickets.find(t => t.id === ticketId);
      if (ticket) {
        ticket.operadorAsignadoId = operadorId || null;
        // Note: In a real scenario, we might want to fetch the operator name or 
        // have it passed here to update ticket.operadorAsignadoNombre
      }

      if (this.selectedTicket?.id === ticketId) {
        this.selectedTicket = { ...this.selectedTicket, operadorAsignadoId: operadorId || null };
      }
    },

    selectTicket(ticket) {
      this.selectedTicket = ticket;
      this.comments = safeArray(ticket?._comments);
    },

    clearSelection() {
      this.selectedTicket = null;
      this.comments = [];
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
    byStatus: (state) => (status) => state.tickets.filter((ticket) => ticket.estado === status),
    stats: (state) => ({
      total: state.tickets.length,
      pending: state.tickets.filter((ticket) => ticket.estado === 'Pendiente' || ticket.estado === 0).length,
      inProgress: state.tickets.filter((ticket) => ticket.estado === 'EnProceso' || ticket.estado === 1).length,
      resolved: state.tickets.filter((ticket) => ticket.estado === 'Resuelto' || ticket.estado === 2).length,
      closed: state.tickets.filter((ticket) => ticket.estado === 'Cerrado' || ticket.estado === 3).length,
      waiting: state.tickets.filter((ticket) => ticket.estado === 'EsperandoUsuario' || ticket.estado === 4).length,
      urgent: state.tickets.filter((ticket) => ticket.prioridad === 'Alta' || ticket.prioridad === 2).length,
    })
  }
});
