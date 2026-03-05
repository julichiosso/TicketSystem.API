<template>
  <div class="flex flex-col w-full h-full"
    :class="settingsStore.isDark ? 'bg-slate-900' : 'bg-white'">

    <!-- Messages area -->
    <div ref="messagesContainer"
      class="flex-1 overflow-y-auto space-y-2 mb-3 custom-scrollbar px-1">

      <div v-if="messages.length === 0"
        class="flex flex-col items-center justify-center h-48 space-y-3">
        <MessageSquareIcon class="w-8 h-8 opacity-20"
          :class="settingsStore.isDark ? 'text-slate-500' : 'text-slate-400'" />
        <p class="text-xs font-bold uppercase tracking-widest"
          :class="settingsStore.isDark ? 'text-slate-600' : 'text-slate-400'">
          Sin actividad aún
        </p>
      </div>

      <div v-for="(msg, index) in messages" :key="msg.id">

        <!-- Separador de fecha -->
        <div v-if="showDateSeparator(index)"
          class="flex items-center gap-3 my-3">
          <div class="flex-1 h-px"
            :class="settingsStore.isDark ? 'bg-slate-800' : 'bg-slate-100'"></div>
          <span class="text-[9px] font-black uppercase tracking-widest"
            :class="settingsStore.isDark ? 'text-slate-600' : 'text-slate-400'">
            {{ formatDateSeparator(msg.timestamp) }}
          </span>
          <div class="flex-1 h-px"
            :class="settingsStore.isDark ? 'bg-slate-800' : 'bg-slate-100'"></div>
        </div>

        <div :class="esPropio(msg) ? 'flex justify-end' : 'flex justify-start'">

          <!-- Nota interna -->
          <div v-if="msg.interno"
            class="w-full max-w-sm rounded-xl border border-amber-500/20 bg-amber-500/5 px-3 py-2 relative mt-2">
            <span class="absolute -top-2 right-2 bg-amber-500 text-[8px] font-black text-black px-2 py-0.5 rounded-full uppercase tracking-wider">
              Nota Interna
            </span>
            <p class="text-[10px] font-semibold text-amber-400/70 mb-1">{{ msg.autor }}</p>
            <p class="text-sm text-amber-200">{{ msg.texto }}</p>
            <p class="text-[9px] text-amber-500/40 mt-1 font-mono">{{ formatTime(msg.timestamp) }}</p>
          </div>

          <!-- Mensaje propio -->
          <div v-else-if="esPropio(msg)" class="flex flex-col items-end gap-0.5 max-w-xs">
            <div class="px-4 py-2.5 rounded-2xl rounded-tr-sm bg-blue-600 text-white">
              <p class="text-sm leading-relaxed">{{ msg.texto }}</p>
            </div>
            <p class="text-[9px] font-mono px-1"
              :class="settingsStore.isDark ? 'text-slate-600' : 'text-slate-400'">
              {{ formatTime(msg.timestamp) }}
            </p>
          </div>

          <!-- Mensaje ajeno -->
          <div v-else class="flex flex-col items-start gap-0.5 max-w-xs">
            <p class="text-[10px] font-semibold px-1"
              :class="settingsStore.isDark ? 'text-slate-500' : 'text-slate-400'">
              {{ msg.autor }}
              <span class="uppercase tracking-widest text-[8px] ml-1 px-1.5 py-0.5 rounded-full"
                :class="roleBadgeClass(msg.autorRol)">
                {{ roleLabel(msg.autorRol) }}
              </span>
            </p>
            <div class="px-4 py-2.5 rounded-2xl rounded-tl-sm border"
              :class="settingsStore.isDark
                ? 'bg-slate-800 border-slate-700 text-white'
                : 'bg-slate-50 border-slate-200 text-slate-900'">
              <p class="text-sm leading-relaxed">{{ msg.texto }}</p>
            </div>
            <p class="text-[9px] font-mono px-1"
              :class="settingsStore.isDark ? 'text-slate-600' : 'text-slate-400'">
              {{ formatTime(msg.timestamp) }}
            </p>
          </div>

        </div>
      </div>

      <!-- Indicador escribiendo -->
      <div v-if="alguienEscribe" class="flex justify-start">
        <div class="px-4 py-3 rounded-2xl rounded-tl-sm border"
          :class="settingsStore.isDark ? 'bg-slate-800 border-slate-700' : 'bg-slate-50 border-slate-200'">
          <div class="flex gap-1 items-center">
            <span v-for="i in 3" :key="i"
              class="w-1.5 h-1.5 rounded-full bg-slate-400 animate-bounce"
              :style="`animation-delay: ${(i-1)*0.2}s`"></span>
          </div>
        </div>
      </div>

    </div>

    <!-- Input area -->
    <div class="space-y-2 flex-shrink-0 border-t pt-3"
      :class="settingsStore.isDark ? 'border-slate-800' : 'border-slate-100'">

      <div v-if="authStore.isAdmin || authStore.isOperador"
        class="flex items-center gap-2 px-1">
        <button @click="isInternalMode = !isInternalMode"
          class="flex items-center gap-2 group">
          <div class="w-7 h-3.5 rounded-full relative transition-all"
            :class="isInternalMode
              ? 'bg-amber-500'
              : settingsStore.isDark ? 'bg-slate-700' : 'bg-slate-200'">
            <div class="absolute top-0.5 w-2.5 h-2.5 rounded-full bg-white transition-all shadow-sm"
              :class="isInternalMode ? 'left-[calc(100%-0.75rem)]' : 'left-0.5'"></div>
          </div>
          <span class="text-[10px] font-black uppercase tracking-widest transition-colors"
            :class="isInternalMode
              ? 'text-amber-400'
              : settingsStore.isDark ? 'text-slate-600' : 'text-slate-400'">
            Nota Interna
          </span>
        </button>
      </div>

      <div class="flex gap-2 items-end">
        <textarea
          ref="inputRef"
          v-model="newMessage"
          @keydown.enter.exact.prevent="sendMessage"
          @keydown.enter.shift.exact="newMessage += '\n'"
          @input="autoResize; notificarEscribiendo()"
          rows="1"
          :placeholder="isInternalMode ? 'Nota interna...' : 'Escribe un mensaje...'"
          class="flex-1 border px-4 py-3 rounded-xl text-sm outline-none transition-all font-medium resize-none"
          :class="settingsStore.isDark
            ? 'bg-slate-800 border-slate-700 text-white placeholder-slate-600 focus:border-blue-500'
            : 'bg-slate-50 border-slate-200 text-slate-900 placeholder-slate-400 focus:border-blue-400'"
          style="max-height: 120px; overflow-y: auto;" />
        <button @click="sendMessage" :disabled="!newMessage.trim() || sending"
          class="px-5 py-3 text-white rounded-xl text-xs font-black uppercase tracking-widest transition-all active:scale-95 disabled:opacity-30 disabled:cursor-not-allowed flex-shrink-0"
          :class="isInternalMode ? 'bg-amber-500 hover:bg-amber-400' : 'bg-blue-600 hover:bg-blue-500'">
          <SendIcon v-if="!sending" class="w-4 h-4" />
          <div v-else class="w-4 h-4 border-2 border-white/30 border-t-white rounded-full animate-spin"></div>
        </button>
      </div>

      <p class="text-[9px] px-1"
        :class="settingsStore.isDark ? 'text-slate-700' : 'text-slate-300'">
        Enter para enviar · Shift+Enter para nueva línea
      </p>
    </div>
  </div>
</template>

<script setup>
import { ref, watch, nextTick, onMounted, onUnmounted } from 'vue';
import * as signalR from '@microsoft/signalr';
import { MessageSquareIcon, SendIcon } from 'lucide-vue-next';
import { useTicketsStore } from '../store/tickets';
import { useAuthStore } from '../store/auth';
import { useSettingsStore } from '../store/settings';

const props = defineProps({ ticketId: String });

const ticketsStore  = useTicketsStore();
const authStore     = useAuthStore();
const settingsStore = useSettingsStore();

const messages          = ref([]);
const newMessage        = ref('');
const isInternalMode    = ref(false);
const messagesContainer = ref(null);
const inputRef          = ref(null);
const sending           = ref(false);
const alguienEscribe    = ref(false);
let escribiendoTimer    = null;
let connection          = null;

// ── Formato fechas ────────────────────────────────────────────────────────
const parseDate = (timestamp) => {
  if (!timestamp) return null;
  // Si no tiene Z ni offset, forzar UTC
  const str = timestamp.toString();
  const normalized = str.endsWith('Z') || str.includes('+') ? str : str + 'Z';
  const d = new Date(normalized);
  return isNaN(d.getTime()) ? null : d;
};

const formatTime = (timestamp) => {
  const date = parseDate(timestamp);
  if (!date) return '';
  return new Intl.DateTimeFormat('es-AR', {
    hour: '2-digit', minute: '2-digit',
    timeZone: 'America/Argentina/Buenos_Aires'
  }).format(date);
};

const formatDateSeparator = (timestamp) => {
  const date = parseDate(timestamp);
  if (!date) return '';
  const now       = new Date();
  const today     = new Date(now.toLocaleDateString('en-CA', { timeZone: 'America/Argentina/Buenos_Aires' }));
  const yesterday = new Date(today);
  yesterday.setDate(yesterday.getDate() - 1);
  const dateDay   = new Date(date.toLocaleDateString('en-CA', { timeZone: 'America/Argentina/Buenos_Aires' }));

  if (dateDay.getTime() === today.getTime())     return 'Hoy';
  if (dateDay.getTime() === yesterday.getTime()) return 'Ayer';

  return new Intl.DateTimeFormat('es-AR', {
    day: '2-digit', month: 'long', year: 'numeric',
    timeZone: 'America/Argentina/Buenos_Aires'
  }).format(date);
};

const showDateSeparator = (index) => {
  if (index === 0) return true;
  const prev = parseDate(messages.value[index - 1]?.timestamp);
  const curr = parseDate(messages.value[index]?.timestamp);
  if (!prev || !curr) return false;
  const tz = 'America/Argentina/Buenos_Aires';
  return prev.toLocaleDateString('en-CA', { timeZone: tz }) !==
         curr.toLocaleDateString('en-CA', { timeZone: tz });
};

// ── Roles ─────────────────────────────────────────────────────────────────
const roleLabel = (r) => {
  if (r === null || r === undefined) return '';
  const n = typeof r === 'string' ? parseInt(r, 10) : r;
  return n === 2 ? 'Admin' : n === 1 ? 'Operador' : 'Usuario';
};

const roleBadgeClass = (r) => {
  const n = typeof r === 'string' ? parseInt(r, 10) : r;
  if (n === 2) return 'bg-purple-500/10 text-purple-400';
  if (n === 1) return 'bg-emerald-500/10 text-emerald-400';
  return 'bg-slate-500/10 text-slate-400';
};

// ── Propio ────────────────────────────────────────────────────────────────
const esPropio = (msg) => {
  const userId = authStore.user?.id?.toString().toLowerCase();
  const autorId = msg.autorId?.toString().toLowerCase();
  return userId && autorId && userId === autorId;
};

const mapComment = (c) => ({
  id:        c.id,
  autorId:   c.autorId,
  autor:     c.autor || 'Sistema',
  autorRol:  c.autorRol ?? c.rol ?? null,
  texto:     c.mensaje,
  interno:   c.interno,
  timestamp: c.fecha ?? null
});

// ── Scroll ────────────────────────────────────────────────────────────────
const scrollToBottom = (smooth = false) => {
  nextTick(() => {
    if (!messagesContainer.value) return;
    messagesContainer.value.scrollTo({
      top: messagesContainer.value.scrollHeight,
      behavior: smooth ? 'smooth' : 'instant'
    });
  });
};

// ── Textarea ──────────────────────────────────────────────────────────────
const autoResize = () => {
  const el = inputRef.value;
  if (!el) return;
  el.style.height = 'auto';
  el.style.height = Math.min(el.scrollHeight, 120) + 'px';
};

const resetInput = () => {
  newMessage.value = '';
  nextTick(() => {
    if (inputRef.value) {
      inputRef.value.style.height = 'auto';
      inputRef.value.focus();
    }
  });
};

// ── SignalR ───────────────────────────────────────────────────────────────
const getToken = () => {
  // El token está directo en localStorage.token
  return localStorage.getItem('token') ?? '';
};

const conectarSignalR = async () => {
  if (!props.ticketId) return;

  const baseUrl = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5000';

  connection = new signalR.HubConnectionBuilder()
    .withUrl(`${baseUrl}/hubs/tickets`, {
      accessTokenFactory: () => getToken()
    })
    .withAutomaticReconnect()
    .configureLogging(signalR.LogLevel.Warning)
    .build();

  connection.on('NuevoMensaje', (comentario) => {
    const msg = mapComment(comentario);
    if (!messages.value.find(m => m.id === msg.id)) {
      messages.value.push(msg);
      scrollToBottom(true);
    }
  });

  connection.on('UsuarioEscribiendo', () => {
    alguienEscribe.value = true;
    clearTimeout(escribiendoTimer);
    escribiendoTimer = setTimeout(() => { alguienEscribe.value = false; }, 2500);
  });

  try {
    await connection.start();
    await connection.invoke('UnirseATicket', props.ticketId);
    console.log('[SignalR] Conectado al ticket', props.ticketId);
  } catch (e) {
    console.warn('[SignalR] No disponible, usando REST:', e.message);
  }
};

const desconectarSignalR = async () => {
  if (connection) {
    try {
      if (props.ticketId)
        await connection.invoke('SalirDeTicket', props.ticketId);
      await connection.stop();
    } catch { }
    connection = null;
  }
};

const notificarEscribiendo = async () => {
  if (connection?.state === signalR.HubConnectionState.Connected) {
    try {
      await connection.invoke('NotificarEscribiendo', props.ticketId);
    } catch { }
  }
};

// ── Cargar mensajes ───────────────────────────────────────────────────────
const cargarMensajes = async (id) => {
  if (!id) return;
  await ticketsStore.fetchComments(id);
  messages.value = ticketsStore.comments.map(mapComment);
  scrollToBottom(false);
  nextTick(() => inputRef.value?.focus());
};

// ── Watchers ──────────────────────────────────────────────────────────────
watch(() => props.ticketId, async (id) => {
  await desconectarSignalR();
  messages.value = [];
  if (id) {
    await cargarMensajes(id);
    await conectarSignalR();
  }
}, { immediate: true });

// ── Enviar ────────────────────────────────────────────────────────────────
const sendMessage = async () => {
  if (!newMessage.value.trim() || !props.ticketId || sending.value) return;
  const texto   = newMessage.value.trim();
  const interno = isInternalMode.value;
  sending.value = true;
  resetInput();

  try {
    if (connection?.state === signalR.HubConnectionState.Connected) {
      await connection.invoke('EnviarMensaje', props.ticketId, texto, interno);
    } else {
      // Fallback REST
      await ticketsStore.addComment(props.ticketId, { mensaje: texto, interno });
      await cargarMensajes(props.ticketId);
    }
  } catch (e) {
    console.error(e);
    newMessage.value = texto;
  } finally {
    sending.value = false;
  }
};

onMounted(() => {
  nextTick(() => inputRef.value?.focus());
});

onUnmounted(desconectarSignalR);
</script>

<style scoped>
.custom-scrollbar::-webkit-scrollbar { width: 4px; }
.custom-scrollbar::-webkit-scrollbar-track { background: transparent; }
.custom-scrollbar::-webkit-scrollbar-thumb { background: rgba(100,100,100,0.15); border-radius: 10px; }
</style>