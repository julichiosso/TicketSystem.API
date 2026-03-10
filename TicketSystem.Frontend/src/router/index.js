import { createRouter, createWebHistory } from 'vue-router';
import { useAuthStore } from '../store/auth';
const routes = [
  { path: '/', component: () => import('../views/LoginView.vue'), meta: { public: true } },
  { path: '/register', component: () => import('../views/RegisterView.vue'), meta: { public: true } },
  { path: '/forgot-password', component: () => import('../views/ForgotPasswordView.vue'), meta: { public: true } },
  { path: '/reset-password', component: () => import('../views/ResetPasswordView.vue'), meta: { public: true } },
  { path: '/dashboard', alias: ['/tickets'], component: () => import('../views/UserDashboard.vue'), meta: { requiresAuth: true } },
  { path: '/admin', alias: ['/admin/tickets', '/admin/users'], component: () => import('../views/AdminDashboard.vue'), meta: { requiresAuth: true, requiresAdmin: true } },
  { path: '/operator', component: () => import('../views/OperatorDashboard.vue'), meta: { requiresAuth: true, requiresOperator: true } },
  { path: '/settings', component: () => import('../views/SettingsView.vue'), meta: { requiresAuth: true } },
  { path: '/profile', component: () => import('../views/ProfileView.vue'), meta: { requiresAuth: true } },
  { path: '/:pathMatch(.*)*', redirect: '/' }
];
const router = createRouter({
  history: createWebHistory(),
  routes
});
router.beforeEach((to, from, next) => {
  const authStore = useAuthStore();
  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    next('/');
    return;
  }
  if (to.meta.requiresAdmin && !authStore.isAdmin) {
    next(authStore.isOperador ? '/operator' : '/dashboard');
    return;
  }
  if (to.meta.requiresOperator && !authStore.isOperador) {
    next('/dashboard');
    return;
  }
  if (to.path === '/admin' && authStore.isOperador && !authStore.isAdmin) {
    next('/operator');
    return;
  }
  next();
});
export default router;
