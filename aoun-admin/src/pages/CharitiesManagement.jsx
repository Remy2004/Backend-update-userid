import { useState, useEffect } from 'react';
import axios from 'axios';
import { CheckCircle, XCircle } from 'lucide-react';

export default function CharitiesManagement() {
    const [charities, setCharities] = useState([]);

    const fetchCharities = async () => {
        const token = localStorage.getItem('admin_token');
        const res = await axios.get('http://localhost:5055/api/AdminDashboard/charities', { headers: { Authorization: `Bearer ${token}` } });
        setCharities(res.data.data);
    };

    useEffect(() => { fetchCharities(); }, []);

    const changeStatus = async (id, status) => {
        const token = localStorage.getItem('admin_token');
        await axios.put(`http://localhost:5055/api/AdminDashboard/charities/${id}/status`, { status }, { headers: { Authorization: `Bearer ${token}` } });
        fetchCharities();
    };

    return (
        <div className="bg-white p-6 rounded-xl shadow-sm">
            <h3 className="text-xl font-bold mb-4">مراجعة الجمعيات الخيرية</h3>
            <table className="w-full text-right">
                <thead className="bg-gray-100 text-gray-600">
                    <tr><th className="p-3">اسم الجمعية</th><th className="p-3">رقم التسجيل</th><th className="p-3">الحالة</th><th className="p-3">إجراءات</th></tr>
                </thead>
                <tbody>
                    {charities.map(c => (
                        <tr key={c.id} className="border-b">
                            <td className="p-3 font-bold text-teal-700">{c.name}</td>
                            <td className="p-3">{c.registrationNumber}</td>
                            <td className="p-3">
                                <span className={`px-3 py-1 rounded-full text-sm font-bold ${c.status === 'Approved' ? 'bg-green-100 text-green-700' : c.status === 'Rejected' ? 'bg-red-100 text-red-700' : 'bg-yellow-100 text-yellow-700'}`}>
                                    {c.status || 'Pending'}
                                </span>
                            </td>
                            <td className="p-3 flex gap-2">
                                <button onClick={() => changeStatus(c.id, 'Approved')} className="text-green-600 bg-green-50 p-2 rounded-lg hover:bg-green-100"><CheckCircle size={18}/></button>
                                <button onClick={() => changeStatus(c.id, 'Rejected')} className="text-red-600 bg-red-50 p-2 rounded-lg hover:bg-red-100"><XCircle size={18}/></button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
}