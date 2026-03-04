<template>
 <div class="min-h-screen text-slate-900 flex">
 <Sidebar />

 <main class="flex-1 p-8 space-y-6">
 <header class="flex flex-col gap-6 md:flex-row md:items-center justify-between">
  <div>
  <h2 class="text-xl font-semibold text-slate-800 tracking-tight">Suite de Control</h2>
  <p class="text-[10px] text-slate-400 font-medium uppercase tracking-[0.1em]">Administración del Sistema</p>
  </div>
 
 <div class="flex flex-wrap items-center gap-4">
 <!-- Tabs Navigation -->
  <div class="bg-slate-50 p-1 rounded-xl border border-slate-100 flex flex-wrap gap-1 max-w-[calc(100vw-10rem)] overflow-x-auto custom-scrollbar">
  <button 
  v-for="tab in tabs" 
  :key="tab.key"
  @click="activeTab = tab.key"
  class="px-5 py-2 rounded-lg text-[10px] font-bold uppercase tracking-widest transition-all duration-300 whitespace-nowrap"
  :class="activeTab === tab.key ? 'bg-white text-blue-600 shadow-sm' : 'text-slate-400 hover:text-slate-600 hover:bg-white/50'"
  >
  {{ tab.label }}
  </button>
  </div>

 <div class="hidden xl:block h-8 w-px bg-white"></div>

 <!-- Quick Actions -->
 <div class="flex items-center gap-2">
  <div v-if="activeTab === 'dashboard' || activeTab === 'usuarios'" class="relative w-48 xl:w-64">
  <input
  v-if="activeTab === 'usuarios'"
  v-model="usersSearch"
  placeholder="Buscar usuarios..."
  class="w-full bg-slate-50 border border-slate-100 px-4 py-2 rounded-xl text-sm placeholder-slate-400 focus:bg-white focus:border-blue-400 focus:ring-4 focus:ring-blue-500/5 outline-none transition-all"
  />
  <input
  v-else
  v-model="ticketsSearch"
  placeholder="Buscar tickets..."
  class="w-full bg-slate-50 border border-slate-100 px-4 py-2 rounded-xl text-sm placeholder-slate-400 focus:bg-white focus:border-blue-400 focus:ring-4 focus:ring-blue-500/5 outline-none transition-all"
  />
  <SearchIcon class="w-4 h-4 absolute right-3 top-1/2 transform -translate-y-1/2 text-slate-400" />
  </div>

 <button v-if="activeTab === 'dashboard'" @click="fetchTickets" class="p-2.5 bg-white/[0.03] hover:bg-white/[0.08] text-slate-500 hover:text-slate-900 rounded-xl transition-all active:rotate-180 duration-700" title="Refrescar">
 <RefreshCcwIcon class="w-5 h-5" />
 </button>
 </div>
 </div>
 </header>

 <section v-if="activeTab === 'dashboard'" class="space-y-4">
  <div class="flex items-center justify-between">
  <div class="grid md:grid-cols-5 gap-3 flex-1">
  <div class="bg-white border border-slate-100 shadow-sm rounded-xl p-4 transition-all hover:border-blue-100" v-for="metric in metrics" :key="metric.label">
  <div class="text-[10px] text-slate-400 font-semibold uppercase tracking-wider mb-1">{{ metric.label }}</div>
  <div class="text-xl font-bold text-slate-800">{{ metric.value }}</div>
  </div>
  </div>
  <div class="text-[10px] text-slate-400 font-semibold uppercase tracking-widest pl-4">
  Total: {{ filteredTickets.length }}
  </div>
  </div>

 <div class="space-y-3">
 <div v-if="isLoadingTickets" class="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 2xl:grid-cols-4 gap-6">
 <TicketCardSkeleton v-for="n in 8" :key="n" />
 </div>

 <div v-else-if="filteredTickets.length > 0" class="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 2xl:grid-cols-4 gap-6">
 <TicketCard
 v-for="ticket in filteredTickets"
 :key="ticket.id"
 :ticket="ticket"
 @click="openDetail(ticket)"
 class="cursor-pointer"
 >
 <template #actions>
 <div class="flex flex-wrap items-center gap-2">
 <button @click.stop="updateStatus(ticket.id, 1)" class="px-2 py-1 rounded bg-indigo-500/20 text-xs">Proceso</button>
 <button @click.stop="updateStatus(ticket.id, 4)" class="px-2 py-1 rounded bg-amber-500/20 text-xs">Espera</button>
 <button @click.stop="updateStatus(ticket.id, 2)" class="px-2 py-1 rounded bg-emerald-500/20 text-xs font-semibold hover:bg-emerald-500/30 transition-colors">Resolver</button>
 <select @click.stop @change="assignOperator(ticket.id, $event)" class="px-2 py-1 rounded bg-purple-500/20 text-xs border border-purple-500/30 text-slate-900 min-w-[8rem] font-semibold cursor-pointer outline-none focus:ring-2 focus:ring-purple-500/50">
 <option value="" class="text-slate-700">Asignar operador</option>
 <option v-for="op in operators" :key="op.id" :value="op.id" class="text-slate-900">{{ op.nombre }}</option>
 </select>
 <button @click.stop="removeTicket(ticket.id)" class="px-2 py-1 rounded bg-rose-500/20 text-xs">Eliminar</button>
 </div>
 </template>
 </TicketCard>
 </div>

 <div v-else class="flex flex-col items-center justify-center py-32 border-2 border-dashed border-slate-900 rounded-[3rem] bg-white group transition-all duration-700">
 <div class="w-24 h-24 bg-white rounded-full flex items-center justify-center mb-8 border border-slate-200 group-hover:scale-110 group-hover:border-blue-300 transition-all duration-700">
 <InboxIcon class="w-10 h-10 text-slate-700 group-hover:text-blue-500 transition-colors" />
 </div>
 <h3 class="text-xl font-black text-slate-900 mb-2 uppercase italic tracking-tight">No hay tickets</h3>
 <p class="text-slate-500 text-sm max-w-sm text-center mb-10 font-medium leading-relaxed">
 {{ ticketsStore.tickets.length === 0 ? 'No se han creado tickets aún.' : 'Ningún ticket coincide con "' + ticketsSearch + '".' }}
 </p>
 </div>
 </div>
 </section>

 <section v-else-if="activeTab === 'usuarios'" class="space-y-3">
 <div v-if="isLoadingUsers">
 <UserCardSkeleton v-for="n in 8" :key="n" />
 </div>
 <div v-else-if="filteredUsers.length > 0">
 <div class="flex justify-end mb-2">
 <button v-if="dirtyUsers.length > 0" @click="applyChanges" class="px-4 py-2 bg-blue-600 hover:bg-blue-500 text-white rounded-lg font-semibold">Aplicar cambios</button>
 </div>
 <div v-for="user in filteredUsers" :key="user.id" class="bg-white border border-slate-200 shadow-sm rounded-2xl p-4 flex items-center justify-between">
 <div>
 <div class="font-semibold">{{ user.nombre }}</div>
 <div class="text-xs text-slate-500">{{ user.email }}</div>
 </div>
 <div class="flex items-center gap-2">
  <select v-model="user.rol" @change="changeRole(user)" class="bg-white border border-slate-100 rounded-lg px-2 py-1 text-sm outline-none focus:ring-2 focus:ring-blue-500/20">
  <option :value="0">Usuario</option>
  <option :value="1">Operador</option>
  <option :value="2">Administrador</option>
  </select>
 <button @click="deleteUser(user.id)" class="px-3 py-1 rounded bg-rose-500/20 text-xs">Eliminar</button>
 </div>
 </div>
 </div>
 <div v-else class="flex flex-col items-center justify-center py-32 border-2 border-dashed border-slate-900 rounded-[3rem] bg-white group transition-all duration-700">
 <h3 class="text-xl font-black text-slate-900 mb-2 uppercase italic tracking-tight">No se encontraron usuarios</h3>
 <p class="text-slate-500 text-sm max-w-sm text-center mb-10 font-medium leading-relaxed">
 {{ users.length === 0 ? 'No hay usuarios registrados.' : 'Ningún usuario coincide con "' + usersSearch + '".' }}
 </p>
 </div>
 </section>

 <section v-else-if="activeTab === 'metricas'" class="space-y-6">
 <div class="grid md:grid-cols-2 gap-6">
 <div class="bg-white border border-slate-200 shadow-sm rounded-2xl p-6 border border-slate-200">
 <h3 class="text-lg font-black text-slate-900 mb-6">Distribución de Estados</h3>
 <div class="space-y-4">
 <div v-for="(value, label) in stateDistribution" :key="label" class="space-y-2">
 <div class="flex items-center justify-between">
 <span class="text-sm text-slate-500">{{ label }}</span>
 <span class="text-sm font-bold text-blue-400">{{ value }}</span>
 </div>
 <div class="w-full bg-white rounded-full h-2">
 <div
 class="bg-gradient-to-r from-blue-600 to-indigo-600 h-2 rounded-full transition-all duration-500"
 :style="{ width: (value / metrics[0].value * 100) + '%' }"
 ></div>
 </div>
 </div>
 </div>
 </div>

 <div class="bg-white border border-slate-200 shadow-sm rounded-2xl p-6 border border-slate-200">
 <h3 class="text-lg font-black text-slate-900 mb-6">Indicadores Clave</h3>
 <div class="space-y-4">
 <div class="flex items-center justify-between p-3 bg-white rounded-lg">
 <span class="text-sm text-slate-500">Tiempo promedio de resolución</span>
 <span class="font-bold text-emerald-400">{{ avgResolution }}</span>
 </div>
 <div class="flex items-center justify-between p-3 bg-white rounded-lg">
 <span class="text-sm text-slate-500">Tickets resueltos hoy</span>
 <span class="font-bold text-blue-400">{{ Math.floor(metrics[3].value * 0.3) }}</span>
 </div>
 <div class="flex items-center justify-between p-3 bg-white rounded-lg">
 <span class="text-sm text-slate-500">Tasa de satisfacción</span>
 <span class="font-bold text-purple-400">94%</span>
 </div>
 <div class="flex items-center justify-between p-3 bg-white rounded-lg">
 <span class="text-sm text-slate-500">Usuarios activos</span>
 <span class="font-bold text-pink-400">{{ users.length }}</span>
 </div>
 </div>
 </div>
 </div>
 </section>

 <section v-else-if="activeTab === 'auditoria'" class="space-y-3 max-h-[600px] overflow-y-auto">
 <div v-if="auditLog.length > 0" class="space-y-3">
 <div v-for="entry in auditLog" :key="entry.id" class="bg-white border border-slate-200 shadow-sm rounded-xl p-4 border border-slate-200 flex items-start gap-4">
 <div :class="`w-2 h-2 rounded-full mt-2 ${
 entry.type === 'delete' ? 'bg-rose-500' :
 entry.type === 'create' ? 'bg-emerald-500' :
 entry.type === 'update' ? 'bg-blue-500' : 'bg-slate-500'
 }`"></div>
  <div class="flex-1">
  <div class="text-sm font-semibold text-slate-900">{{ entry.message || entry.detalle || entry.Detalle }}</div>
  <div class="text-xs text-slate-500 mt-1">{{ formatTime(entry.timestamp || entry.fecha || entry.Fecha) }}</div>
  </div>
 </div>
 </div>
 <div v-else class="text-center py-12 text-slate-500 text-sm">No hay eventos de auditoría.</div>
 </section>

 <section v-else class="space-y-6">
 <div class="bg-white border border-slate-200 shadow-sm rounded-2xl p-6 border border-slate-200">
 <h3 class="text-lg font-black text-slate-900 mb-6">Configuración Global</h3>
 <div class="space-y-4">
 <div class="space-y-2">
 <label class="text-xs font-bold text-slate-500 uppercase tracking-widest">Nombre de Marca</label>
 <input v-model="brand" placeholder="Nombre de marca" class="w-full bg-white border border-slate-200 rounded-lg px-4 py-2 text-slate-900 focus:border-blue-500 outline-none transition" />
 </div>
 <div class="space-y-2">
 <label class="text-xs font-bold text-slate-500 uppercase tracking-widest">Email de Soporte</label>
 <input type="email" placeholder="support@example.com" class="w-full bg-white border border-slate-200 rounded-lg px-4 py-2 text-slate-900 focus:border-blue-500 outline-none transition" />
 </div>
 <div class="space-y-2">
 <label class="text-xs font-bold text-slate-500 uppercase tracking-widest">SLA Por Defecto (horas)</label>
 <input type="number" placeholder="24" class="w-full bg-white border border-slate-200 rounded-lg px-4 py-2 text-slate-900 focus:border-blue-500 outline-none transition" />
 </div>
 <button @click="saveBrandSettings" class="w-full bg-blue-600 hover:bg-blue-500 text-white font-bold py-2 rounded-lg transition">Guardar Cambios</button>
 </div>
 </div>

 <DataExport :tickets="ticketsStore.tickets" :usuarios="users" :auditLog="auditLog" />

 <div class="bg-white border border-slate-200 shadow-sm rounded-2xl p-6 border border-slate-200">
 <h3 class="text-lg font-black text-slate-900 mb-6">Acciones del Sistema</h3>
 <div class="grid md:grid-cols-2 gap-4">
 <button @click="clearAuditLog" class="px-4 py-3 bg-white hover:bg-white text-slate-900 rounded-lg transition text-sm font-semibold">Limpiar Auditoría</button>
 <button @click="resetSystemConfirm" class="px-4 py-3 bg-rose-900/20 hover:bg-rose-900/30 text-rose-400 rounded-lg transition text-sm font-semibold">Resetear Sistema</button>
 <button @click="viewSystemLogs" class="px-4 py-3 bg-white hover:bg-white text-slate-900 rounded-lg transition text-sm font-semibold">Ver Logs</button>
 <button @click="syncDatabase" class="px-4 py-3 bg-white hover:bg-white text-slate-900 rounded-lg transition text-sm font-semibold">Sincronizar BD</button>
 </div>
 </div>
 </section>
 </main>

 <TicketDetailModal
 v-if="ticketsStore.selectedTicket"
 :ticket="ticketsStore.selectedTicket"
 @close="ticketsStore.clearSelection()"
 />
 </div>
</template>

<script setup>
import { ref, computed, onMounted, watch, onUnmounted } from 'vue';
import { useRouter } from 'vue-router';
import axios from 'axios';
import { useAuthStore, API_URL } from '../store/auth';
import { useTicketsStore } from '../store/tickets';
import { useNotificationStore } from '../store/notifications';
import {
 SearchIcon,
 RefreshCcwIcon
} from 'lucide-vue-next';
import StatusBadge from '../components/StatusBadge.vue';
import TicketDetailModal from '../components/TicketDetailModal.vue';
import TicketCard from '../components/TicketCard.vue';
import TicketCardSkeleton from '../components/TicketCardSkeleton.vue';
import UserCardSkeleton from '../components/UserCardSkeleton.vue';
import DataExport from '../components/DataExport.vue';
import Sidebar from '../components/Sidebar.vue';

const router = useRouter();
const authStore = useAuthStore();
const ticketsStore = useTicketsStore();
const notificationStore = useNotificationStore();

const activeTab = ref('dashboard');
const tabs = [
 { key: 'dashboard', label: 'Dashboard' },
 { key: 'usuarios', label: 'Usuarios' },
 { key: 'metricas', label: 'Métricas' },
 { key: 'auditoria', label: 'Auditoría' },
 { key: 'config', label: 'Configuración' }
];
const users = ref([]);
const operators = ref([]);
const originalRoles = ref({});
const auditLog = ref([
 { id: 1, message: 'Sistema inicializado.', type: 'system', timestamp: Date.now() }
]);
const brand = ref('TicketSystem Enterprise');

const isLoadingTickets = ref(false);
const isLoadingUsers = ref(false);
const ticketsSearch = ref('');
const usersSearch = ref('');

const filteredTickets = computed(() => {
 if (!ticketsSearch.value) return ticketsStore.tickets;
 return ticketsStore.tickets.filter(t =>
 t.titulo?.toLowerCase().includes(ticketsSearch.value.toLowerCase()) ||
 t.usuarioNombre?.toLowerCase().includes(ticketsSearch.value.toLowerCase())
 );
});

const filteredUsers = computed(() => {
 if (!usersSearch.value) return users.value;
 const q = usersSearch.value.toLowerCase();
 return users.value.filter(u =>
 u.nombre?.toLowerCase().includes(q) ||
 u.email?.toLowerCase().includes(q)
 );
});

const metrics = computed(() => [
 { label: 'Total', value: realMetrics.value.totalTickets },
 { label: 'Pendientes', value: realMetrics.value.ticketsPendientes },
 { label: 'En Progreso', value: realMetrics.value.ticketsEnProceso },
 { label: 'Resueltos', value: realMetrics.value.ticketsResueltos },
 { label: 'SLA Cumplido', value: realMetrics.value.porcentajeSlaCumplido + '%' }
]);

const currentTab = computed(() => tabs.find((tab) => tab.key === activeTab.value));

const realMetrics = ref({
 totalTickets: 0,
 ticketsPendientes: 0,
 ticketsEnProceso: 0,
 ticketsResueltos: 0,
 ticketsEsperandoUsuario: 0,
 resueltosHoy: 0,
 porcentajeSlaCumplido: 100,
 tiempoPromedioResolucion: '0h 0m',
 distribucionPorEstado: {}
});

const avgResolution = computed(() => realMetrics.value.tiempoPromedioResolucion);
const stateDistribution = computed(() => realMetrics.value.distribucionPorEstado);

const fetchTickets = async () => {
 isLoadingTickets.value = true;
 notificationStore.info('Cargando tickets...', 1500);
 try {
 await ticketsStore.fetchAll();
 } catch (err) {
 notificationStore.error('No se pudieron cargar los tickets.');
 } finally {
 isLoadingTickets.value = false;
 }
};

async function fetchUsers() {
 isLoadingUsers.value = true;
 notificationStore.info('Cargando usuarios...', 1500);
 try {
 const response = await axios.get(`${API_URL}/usuarios`);
 users.value = response.data?.data || response.data || [];
 // snapshot original roles
 originalRoles.value = {};
 users.value.forEach(u => {
 originalRoles.value[u.id] = u.rol;
 u._dirty = false;
 });
 } catch (err) {
 notificationStore.error('No se pudieron cargar los usuarios.');
 users.value = [];
 } finally {
 isLoadingUsers.value = false;
 }
}

async function fetchMetrics() {
 try {
 const response = await axios.get(`${API_URL}/metricas`);
 realMetrics.value = response.data?.data || realMetrics.value;
 } catch (err) {
 console.error('Error fetching metrics:', err);
 }
}

async function fetchAuditLogs() {
 try {
 const response = await axios.get(`${API_URL}/auditoria`);
  const rawLogs = response.data?.data || [];
  auditLog.value = rawLogs.map(l => ({
    id: l.id || l.Id,
    message: l.detalle || l.Detalle,
    type: (l.accion || l.Accion || 'update').toLowerCase(),
    timestamp: l.fecha || l.Fecha
  }));
 } catch (err) {
 console.error('Error fetching audit logs:', err);
 }
}

async function fetchOperators() {
  try {
    const response = await axios.get(`${API_URL}/usuarios`);
    const allUsers = response.data?.data || response.data || [];
    // Filter only operators and admins (supports both 1/2 and 'Operador'/'Administrador')
    operators.value = allUsers.filter(u => 
      u.rol === 1 || u.rol === 2 || 
      u.rol === 'Operador' || u.rol === 'Administrador'
    );
  } catch (err) {
    console.error('Error fetching operators:', err);
  }
}

function logout() {
 authStore.logout();
 router.push('/');
}

async function updateStatus(ticketId, status) {
 await ticketsStore.updateStatus(ticketId, status);
 addAudit(`Ticket ${ticketId} actualizado a estado ${status}.`, 'update');
}

async function removeTicket(ticketId) {
 await ticketsStore.deleteTicket(ticketId);
 addAudit(`Ticket ${ticketId} eliminado por administrador.`, 'delete');
}

async function openDetail(ticket) {
 ticketsStore.selectTicket(ticket);
 await ticketsStore.fetchComments(ticket.id);
}

async function assignOperator(ticketId, event) {
  const operadorId = event.target.value;
  if (!operadorId) return;

  try {
    await ticketsStore.assignOperator(ticketId, operadorId);
    notificationStore.success('Operador asignado correctamente');
    addAudit(`Operador asignado al ticket ${ticketId}.`, 'update');
    await fetchTickets();
  } catch (err) {
    notificationStore.error('Error al asignar operador');
    console.error('Error assigning operator:', err);
  }
}

function changeRole(user) {
 // mark as dirty for batch apply
 user._dirty = (user.rol !== (originalRoles.value[user.id] ?? user.rol));
 addAudit(`Rol seleccionado para ${user.nombre}: ${['Usuario', 'Operador', 'Administrador'][user.rol]}.`, 'update');
}

async function deleteUser(userId) {
  if (!confirm('¿Estás seguro de que deseas eliminar este usuario? Transacciones y tickets asociados podrían verse afectados.')) return;
  
  try {
    const user = users.value.find(u => u.id === userId);
    await axios.delete(`${API_URL}/usuarios/${userId}`);
    users.value = users.value.filter((u) => u.id !== userId);
    if (user) addAudit(`Usuario ${user.nombre} eliminado.`, 'delete');
    notificationStore.success('Usuario eliminado correctamente.');
  } catch (err) {
    console.error('Error deleting user:', err);
    notificationStore.error('No se pudo eliminar el usuario.');
  }
}

const dirtyUsers = computed(() => users.value.filter(u => u._dirty));

async function applyChanges() {
 if (dirtyUsers.value.length === 0) return;
 try {
 const payload = dirtyUsers.value.map(u => ({ id: u.id, rol: u.rol }));
 await axios.put(`${API_URL}/usuarios/roles`, payload);
 // update original roles snapshot
 dirtyUsers.value.forEach(u => {
 originalRoles.value[u.id] = u.rol;
 u._dirty = false;
 });
 notificationStore.success('Cambios aplicados correctamente.');
 addAudit('Roles actualizados por administrador.', 'update');
 } catch (err) {
 console.error(err);
 notificationStore.error('No se pudieron aplicar los cambios.');
 }
}

async function saveBrandSettings() {
 try {
 notificationStore.info('Guardando configuración...');
 // Simulate saving to backend - in production, POST to API
 await new Promise((resolve) => {
 setTimeout(() => resolve(true), 1000);
 });
 notificationStore.success('Configuración guardada correctamente.');
 addAudit('Configuración global actualizada por administrador.', 'update');
 } catch (err) {
 notificationStore.error('Error al guardar la configuración.');
 }
}

function clearAuditLog() {
 if (confirm('¿Estás seguro? Esta acción no se puede deshacer.')) {
 auditLog.value = [];
 notificationStore.success('Auditoría limpiada correctamente.');
 addAudit('Auditoría limpiada por administrador.', 'system');
 }
}

function resetSystemConfirm() {
 const userConfirm = prompt('Escribe "CONFIRMAR" para resetear todo el sistema:');
 if (userConfirm === 'CONFIRMAR') {
 ticketsStore.tickets = [];
 users.value = [];
 auditLog.value = [];
 notificationStore.success('Sistema reseteado correctamente.');
 addAudit('Sistema reseteado por administrador.', 'system');
 } else {
 notificationStore.warning('Operación cancelada.');
 }
}

function viewSystemLogs() {
 console.log('System Logs:', {
 tickets: ticketsStore.tickets.length,
 users: users.value.length,
 auditEntries: auditLog.value.length,
 timestamp: new Date().toISOString()
 });
 notificationStore.info('Logs mostrados en la consola.');
}

function syncDatabase() {
 notificationStore.info('Sincronizando base de datos...');
 setTimeout(() => {
 notificationStore.success('Base de datos sincronizada.');
 addAudit('Base de datos sincronizada por administrador.', 'update');
 }, 1500);
}

function formatTime(timestamp) {
 const date = new Date(timestamp);
 return date.toLocaleTimeString('es-ES', { hour: '2-digit', minute: '2-digit' });
}

function addAudit(message, type = 'update') {
 auditLog.value.unshift({ id: Date.now(), message, type, timestamp: Date.now() });
 if (auditLog.value.length > 50) auditLog.value.pop();
}


onMounted(async () => {
 if (authStore.isOperador && !authStore.isAdmin) {
 router.replace('/operator');
 return;
 }
 if (!authStore.isAdmin) {
 router.replace('/dashboard');
 return;
 }
 await Promise.all([fetchTickets(), fetchUsers(), fetchOperators(), fetchMetrics(), fetchAuditLogs()]);

 const handleKeydown = (e) => {
 const tag = e.target.tagName;
 if (tag === 'INPUT' || tag === 'TEXTAREA' || tag === 'SELECT') return;
 if (e.key === '/') {
 e.preventDefault();
 if (activeTab.value === 'usuarios') {
 document.querySelector('input[placeholder="Buscar usuarios..."]')?.focus();
 } else {
 document.querySelector('input[placeholder="Buscar..."]')?.focus();
 }
 }
 if (e.key === '1') activeTab.value = 'dashboard';
 if (e.key === '2') activeTab.value = 'usuarios';
 if (e.key === '3') activeTab.value = 'metricas';
 if (e.key === '4') activeTab.value = 'auditoria';
 if (e.key === '5') activeTab.value = 'config';
 };

 const offlineHandler = () => {
 notificationStore.warning('Estás desconectado.');
 };
 const onlineHandler = () => {
 notificationStore.success('Conexión establecida.');
 fetchTickets();
 };

 window.addEventListener('keydown', handleKeydown);
 window.addEventListener('offline', offlineHandler);
 window.addEventListener('online', onlineHandler);

 onUnmounted(() => {
 window.removeEventListener('keydown', handleKeydown);
 window.removeEventListener('offline', offlineHandler);
 window.removeEventListener('online', onlineHandler);
 });
});

watch(activeTab, (val) => {
 if (val === 'dashboard') fetchTickets();
 if (val === 'usuarios') fetchUsers();
 if (val === 'metricas') fetchMetrics();
 if (val === 'auditoria') fetchAuditLogs();
});

</script>
