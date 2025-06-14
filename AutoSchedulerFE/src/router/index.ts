import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import ActivitiesView from '@/views/Activities/ActivitiesView.vue'
import ActivityView from '@/views/Activities/ActivityView.vue'
import PreferencesView from '@/views/Activities/PreferencesView.vue'
import GroupsView from '@/views/Groups/GroupsView.vue'
import GroupView from '@/views/Groups/GroupView.vue'
import MembersView from '@/views/Groups/MembersView.vue'
import TimesheetView from '@/views/Timesheets/TimesheetView.vue'
import TimesheetsView from '@/views/Timesheets/TimesheetsView.vue'
import UserView from '@/views/Users/UserView.vue'
import CreateTimesheetView from '@/views/Timesheets/CreateTimesheetView.vue'
import OrganizationView from '@/views/Groups/OrganizationView.vue'
import HallsView from '@/views/Activities/HallsView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: HomeView,
    },
    {
      path: '/about',
      name: 'about',
      // route level code-splitting
      // this generates a separate chunk (About.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      component: () => import('../views/AboutView.vue'),
    },
    {
      path: '/activities',
      name: 'activities',
      component: ActivitiesView,
    },
    {
      path: '/activities/:id',
      name: 'activity',
      component: ActivityView,
    },
    {
      path: '/activities/preferences',
      name: 'preferences',
      component: PreferencesView,
    },
    {
      path: '/activities/halls',
      name: 'halls',
      component: HallsView,
    },
    {
      path: '/groups',
      name: 'groups',
      component: GroupsView,
    },
    {
      path: '/groups/:id',
      name: 'group',
      component: GroupView,
    },
    {
      path: '/groups/members',
      name: 'member',
      component: MembersView,
    },
    {
      path: '/organization/:id',
      name: 'organization',
      component: OrganizationView,
    },
    {
      path: '/timesheets/:id',
      name: 'timesheet',
      component: TimesheetView,
    },
    {
      path: '/timesheets',
      name: 'timesheet',
      component: TimesheetsView,
    },
    {
      path: '/timesheets/create',
      name: 'timesheet',
      component: CreateTimesheetView,
    },
    {
      path: '/user',
      name: 'profile',
      component: UserView,
    },
  ],
})

export default router
