<script setup lang="ts">
import Card from './ui/card/Card.vue';
import CardHeader from './ui/card/CardHeader.vue';
import CardTitle from './ui/card/CardTitle.vue';
import CardDescription from './ui/card/CardDescription.vue';
import CardContent from './ui/card/CardContent.vue';
import { Form } from 'vee-validate';
import { FormField } from './ui/form';
import FormItem from './ui/form/FormItem.vue';
import FormLabel from './ui/form/FormLabel.vue';
import FormControl from './ui/form/FormControl.vue';
import Input from './ui/input/Input.vue';
import Button from './ui/button/Button.vue';
import type { LoginModel } from '@/classes/user';
import { useUserStore } from '@/stores/userStore';
import router from '@/router';

const userStore=useUserStore();

const login:LoginModel={
    email: '',
    password: ''
}

const handleSubmit = ()=>{
    userStore.register(login);
    router.push('login');
}
</script>

<template>
    <Card>
        <CardHeader>
            <CardTitle>Sign up</CardTitle>
            <CardDescription>Create a new account</CardDescription>
        </CardHeader>
        <CardContent>
            <Form @submit="handleSubmit">
                <FormField name="name">
                    <FormItem>
                        <FormLabel>Email address</FormLabel>
                        <FormControl>
                            <Input v-model="login.email" required type="text" placeholder="me@mail.com"/>
                        </FormControl>
                    </FormItem>
                </FormField>
                <FormField name="description">
                    <FormItem>
                        <FormLabel>Password</FormLabel>
                        <FormControl>
                            <Input v-model="login.password" required type="password" placeholder="Enter a Password"/>
                        </FormControl>
                    </FormItem>
                </FormField>
                <Button type="submit">Register</Button>
                <RouterLink :to="`/login`">Sign in</RouterLink>
            </Form>
        </CardContent>
    </Card>
</template>