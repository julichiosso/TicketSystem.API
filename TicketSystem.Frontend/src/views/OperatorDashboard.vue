<template>
 <div class="min-h-screen text-slate-900 flex">
 <Sidebar />

 <main class="flex-1 p-8 space-y-6">
 <header class="flex flex-col gap-6 md:flex-row md:items-center justify-between">
  <div>
  <h2 class="text-xl font-semibold text-slate-800 tracking-tight">Panel Superior</h2>
  <p class="text-[10px] text-slate-400 font-medium uppercase tracking-[0.1em]">Operador de Resoluciones</p>
  </div>
 
 <div class="flex flex-wrap items-center gap-4">
 <!-- Tabs -->
  <div class="bg-slate-50 p-1 rounded-xl border border-slate-100 flex">
  <button 
  v-for="tab in tabs" 
  :key="tab.key"
  @click="activeTab = tab.key"
  class="px-5 py-2 rounded-lg text-[10px] font-bold uppercase tracking-widest transition-all duration-300"
  :class="activeTab === tab.key ? 'bg-white text-blue-600 shadow-sm' : 'text-slate-400 hover:text-slate-600 hover:bg-white/50'"
  >
  {{ tab.label }}
  </button>
  </div>

 <div class="hidden sm:block h-8 w-px bg-white"></div>

 <button @click="fetchTickets" class="p-2.5 bg-white/[0.03] hover:bg-white/[0.08] text-slate-500 hover:text-slate-900 rounded-xl transition-all active:rotate-180 duration-700" title="Refrescar Tickets">
 <RefreshCcwIcon class="w-5 h-5" />
 </button>
 </div>
 </header>

 <section v-if="activeTab === 'tickets'" class="space-y-4">
 <div class="grid md:grid-cols-5 gap-3">
 <div class="bg-white border border-slate-200 shadow-sm rounded-2xl p-4" v-for="metric in metrics" :key="metric.label">
 <div class="text-xs text-slate-500">{{ metric.label }}</div>
 <div class="text-2xl font-bold">{{ metric.value }}</div>
 </div>
 </div>

 <div class="space-y-3">
 <div v-if="isLoadingTickets" class="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 2xl:grid-cols-4 gap-6">
 <TicketCardSkeleton v-for="n in 6" :key="n" />
 </div>

 <div v-else-if="assignedTickets.length > 0" class="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 2xl:grid-cols-4 gap-6">
 <TicketCard
 v-for="ticket in assignedTickets"
 :key="ticket.id"
 :ticket="ticket"
 @click="openDetail(ticket)"
 class="cursor-pointer"
 >
 <template #actions>
 <div class="flex items-center gap-2">
 <button @click.stop="updateStatus(ticket.id, 1)" class="px-2 py-1 rounded bg-indigo-500/20 text-xs">Proceso</button>
 <button @click.stop="updateStatus(ticket.id, 4)" class="px-2 py-1 rounded bg-amber-500/20 text-xs">Espera</button>
 <button @click.stop="updateStatus(ticket.id, 2)" class="px-2 py-1 rounded bg-emerald-500/20 text-xs">Resolver</button>
 </div>
 </template>
 </TicketCard>
 </div>

 <div v-else class="flex flex-col items-center justify-center py-32 border-2 border-dashed border-slate-200 rounded-[3rem] bg-white group transition-all duration-700">
 <div class="w-24 h-24 bg-white rounded-full flex items-center justify-center mb-8 border border-slate-200 group-hover:scale-110 group-hover:border-blue-300 transition-all duration-700">
 <InboxIcon class="w-10 h-10 text-slate-700 group-hover:text-blue-500 transition-colors" />
 </div>
 <h3 class="text-xl font-black text-slate-900 mb-2 uppercase italic tracking-tight">Sin tickets asignados</h3>
 <p class="text-slate-500 text-sm max-w-sm text-center mb-10 font-medium leading-relaxed">
 Aún no tienes tickets asignados. El administrador los asignará pronto.
 </p>
 </div>
 </div>
 </section>

 <section v-else-if="activeTab === 'stats'" class="space-y-6">
 <div class="grid md:grid-cols-2 gap-6">
 <div class="bg-white border border-slate-200 shadow-sm rounded-2xl p-6 border border-slate-200">
 <h3 class="text-lg font-black text-slate-900 mb-6">Mis Estadísticas</h3>
 <div class="space-y-4">
 <div class="flex items-center justify-between p-3 bg-white rounded-lg">
 <span class="text-sm text-slate-500">Tickets totales</span>
 <span class="font-bold text-blue-400">{{ metrics.find(m => m.label === 'Total')?.value || 0 }}</span>
 </div>
 <div class="flex items-center justify-between p-3 bg-white rounded-lg">
 <span class="text-sm text-slate-500">En progreso</span>
 <span class="font-bold text-indigo-400">{{ metrics.find(m => m.label === 'En Progreso')?.value || 0 }}</span>
 </div>
 <div class="flex items-center justify-between p-3 bg-white rounded-lg">
 <span class="text-sm text-slate-500">Resueltos</span>
 <span class="font-bold text-emerald-400">{{ metrics.find(m => m.label === 'Resueltos')?.value || 0 }}</span>
 </div>
 <div class="flex items-center justify-between p-3 bg-white rounded-lg">
 <span class="text-sm text-slate-500">Tasa de resolución</span>
 <span class="font-bold text-purple-400">{{ resolutionRate }}%</span>
 </div>
 </div>
 </div>

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
 :style="{ width: (value / (metrics.find(m => m.label === 'Total')?.value || 1) * 100) + '%' }"
 ></div>
 </div>
 </div>
 </div>
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
import { ref, computed, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import axios from 'axios';
import { useAuthStore, API_URL } from '../store/auth';
import { useTicketsStore } from '../store/tickets';
import {
 RefreshCcwIcon,
 InboxIcon
} from 'lucide-vue-next';
import TicketDetailModal from '../components/TicketDetailModal.vue';
import TicketCard from '../components/TicketCard.vue';
import TicketCardSkeleton from '../components/TicketCardSkeleton.vue';
import Sidebar from '../components/Sidebar.vue';

const router = useRouter();
const authStore = useAuthStore();
const ticketsStore = useTicketsStore();

const activeTab = ref('tickets');
const tabs = [
 { key: 'tickets', label: 'Mis Tickets' },
 { key: 'stats', label: 'Estadísticas' }
];

const assignedTickets = ref([]);
const isLoadingTickets = ref(false);

const metrics = computed(() => {
 const total = assignedTickets.value.length;
 const inProgress = assignedTickets.value.filter(t => t.estado === 1).length;
 const resolved = assignedTickets.value.filter(t => t.estado === 2).length;
 const waiting = assignedTickets.value.filter(t => t.estado === 4).length;
 
 return [
 { label: 'Total', value: total },
 { label: 'En Progreso', value: inProgress },
 { label: 'Esperando', value: waiting },
 { label: 'Resueltos', value: resolved }
 ];
});

const currentTab = computed(() => tabs.find((tab) => tab.key === activeTab.value));

const resolutionRate = computed(() => {
 const total = metrics.value[0].value;
 const resolved = metrics.value[3].value;
 return total > 0 ? Math.round((resolved / total) * 100) : 0;
});

const stateDistribution = computed(() => ({
 'En Progreso': metrics.value.find(m => m.label === 'En Progreso')?.value || 0,
 'Esperando Usuario': metrics.value.find(m => m.label === 'Esperando')?.value || 0,
 'Resueltos': metrics.value.find(m => m.label === 'Resueltos')?.value || 0
}));

const fetchTickets = async () => {
 isLoadingTickets.value = true;
 try {
 const response = await axios.get(`${API_URL}/tickets/operador/mis-tickets`);
 assignedTickets.value = response.data?.data || [];
 } catch (err) {
 console.error('Error fetching operator tickets:', err);
 } finally {
 isLoadingTickets.value = false;
 }
};

const logout = () => {
 authStore.logout();
 router.push('/');
};

const updateStatus = async (ticketId, status) => {
 try {
 await axios.patch(`${API_URL}/tickets/${ticketId}/estado`, status);
 await fetchTickets();
 } catch (err) {
 console.error('Error updating ticket status:', err);
 }
};

const openDetail = async (ticket) => {
 ticketsStore.selectTicket(ticket);
 await ticketsStore.fetchComments(ticket.id);
};

onMounted(() => {
 if (!authStore.isOperador) {
 router.push('/');
 }
 fetchTickets();
});
</script>

<style scoped>
/* Scoped styles kept minimal for clean inheritance from global style.css */
</style>
